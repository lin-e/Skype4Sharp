using System.Net;

namespace Skype4Sharp
{
    public class Skype4Sharp
    {
        public event Events.MessageReceived messageReceived;
        public event Events.ContactReceived contactReceived;
        public event Events.MessageEdited messageEdited;
        public event Events.ChatMembersChanged chatMembersChanged;
        public event Events.TopicChange topicChange;
        public event Events.ContactRequestReceived contactRequestReceived;
        public event Events.CallStarted callStarted;
        public event Events.FileReceived fileReceived;
        public event Events.ChatPictureChanged chatPictureChanged;

        public Auth.SkypeCredentials authInfo;
        public Auth.Tokens authTokens = new Auth.Tokens();
        public User selfProfile;
        public Helpers.WebRequestFactory mainFactory;
        public Enums.LoginState authState = Enums.LoginState.Unknown;
        public Enums.SkypeTokenType tokenType = Enums.SkypeTokenType.Standard;
        public bool ignoreOwnEvents = true;

        private Events.Poller mainPoll;
        private WebProxy mainProxy;
        private Skype4SharpCore.UserModule mainUserModule;
        private Skype4SharpCore.AuthModule mainAuthModule;
        private Skype4SharpCore.MessageModule mainMessageModule;
        private Skype4SharpCore.ContactModule mainContactModule;
        public Skype4Sharp(Auth.SkypeCredentials loginData, WebProxy loginProxy = null)
        {
            authInfo = loginData;
            mainProxy = loginProxy;
            mainFactory = new Helpers.WebRequestFactory(mainProxy, new CookieContainer());
            mainPoll = new Events.Poller(this);
            selfProfile = new User(this);
            mainUserModule = new Skype4SharpCore.UserModule(this);
            mainAuthModule = new Skype4SharpCore.AuthModule(this);
            mainMessageModule = new Skype4SharpCore.MessageModule(this);
            mainContactModule = new Skype4SharpCore.ContactModule(this);
        }
        public void StartPoll()
        {
            blockUnauthorized();
            mainPoll.StartPoll();
        }
        public bool Login()
        {
            if (authState == Enums.LoginState.Success)
            {
                throw new Exceptions.InvalidSkypeActionException("You are already signed in");
            }
            return mainAuthModule.Login();
        }
        public ChatMessage SendMessage(Chat targetChat, string newMessage, Enums.MessageType messageType = Enums.MessageType.Text)
        {
            blockUnauthorized();
            return mainMessageModule.createMessage(targetChat, newMessage, messageType);
        }
        public ChatMessage SendMessage(string targetUser, string newMessage, Enums.MessageType messageType = Enums.MessageType.Text)
        {
            blockUnauthorized();
            Chat targetChat = new Chat(this);
            targetChat.ID = "8:" + targetUser.ToLower();
            targetChat.ChatLink = "https://db3-client-s.gateway.messenger.live.com/v1/users/ME/conversations/" + targetChat.ID;
            targetChat.Type = Enums.ChatType.Private;
            return mainMessageModule.createMessage(targetChat, newMessage, messageType);
        }
        public User GetUser(string inputName)
        {
            blockUnauthorized();
            return mainUserModule.getUser(inputName);
        }
        public void AddUser(string targetUser, string requestMessage)
        {
            blockUnauthorized();
            mainContactModule.addUser(targetUser, requestMessage);
        }
        public void RemoveUser(string targetUser)
        {
            blockUnauthorized();
            mainContactModule.deleteUser(targetUser);
        }
        public User[] GetContacts()
        {
            blockUnauthorized();
            return mainContactModule.getContacts();
        }

        public void editMessage(ChatMessage originalMessage, string newMessage)
        {
            mainMessageModule.editMessage(originalMessage, newMessage);
        }
        public User[] getUsers(string[] inputNames)
        {
            blockUnauthorized();
            return mainUserModule.getUsers(inputNames);
        }
        public void invokeMessageReceived(ChatMessage pMessage)
        {
            if (ignoreOwnEvents)
            {
                if (pMessage.Sender.Username == selfProfile.Username)
                {
                    return;
                }
            }
            try
            {
                messageReceived.Invoke(pMessage);
            }
            catch { }
        }
        public void invokeContactReceived(User receivedUser, Chat originChat, User originUser)
        {
            if (ignoreOwnEvents)
            {
                if (originUser.Username == selfProfile.Username)
                {
                    return;
                }
            }
            try
            {
                contactReceived.Invoke(receivedUser, originChat, originUser);
            }
            catch { }
        }
        public void invokeMessageEdited(ChatMessage pMessage)
        {
            if (ignoreOwnEvents)
            {
                if (pMessage.Sender.Username == selfProfile.Username)
                {
                    return;
                }
            }
            try
            {
                messageEdited.Invoke(pMessage);
            }
            catch { }
        }
        public void invokeChatMembersChanged(Chat newChat, User eventInitiator, User eventTarget, Enums.ChatMemberChangedType changeType)
        {
            try
            {
                chatMembersChanged.Invoke(newChat, eventInitiator, eventTarget, changeType);
            }
            catch { }
        }
        public void invokeTopicChange(Chat targetChat, User eventInitiator, string newTopic)
        {
            try
            {
                topicChange.Invoke(targetChat, eventInitiator, newTopic);
            }
            catch { }
        }
        public void invokeContactRequestReceived(Events.ContactRequest sentRequest)
        {
            try
            {
                contactRequestReceived.Invoke(sentRequest);
            }
            catch { }
        }
        public void invokeCallStarted(Chat originChat, User eventInitiator)
        {
            try
            {
                callStarted.Invoke(originChat, eventInitiator);
            } catch { }
        }
        public void invokeFileReceived(Events.SkypeFile sentFile)
        {
            try
            {
                fileReceived.Invoke(sentFile);
            }
            catch { }
        }
        public void invokeChatPictureChanged(Chat targetChat, User eventInitiator, string newPicture)
        {
            try
            {
                chatPictureChanged.Invoke(targetChat, eventInitiator, newPicture);
            }
            catch { }
        }
        private void blockUnauthorized()
        {
            if (authState != Enums.LoginState.Success)
            {
                throw new Exceptions.InvalidSkypeActionException("Not signed in");
            }
        }
    }
}