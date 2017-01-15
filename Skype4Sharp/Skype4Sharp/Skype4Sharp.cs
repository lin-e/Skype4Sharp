using System.Net;
using Skype4Sharp.Auth;
using Skype4Sharp.Enums;
using Skype4Sharp.Events;
using Skype4Sharp.Exceptions;
using Skype4Sharp.Helpers;
using Skype4Sharp.Skype4SharpCore;

namespace Skype4Sharp
{
    public class Skype4Sharp
    {
        public event MessageReceived messageReceived;
        public event ContactReceived contactReceived;
        public event MessageEdited messageEdited;
        public event ChatMembersChanged chatMembersChanged;
        public event TopicChange topicChange;
        public event ContactRequestReceived contactRequestReceived;
        public event CallStarted callStarted;
        public event FileReceived fileReceived;
        public event ChatPictureChanged chatPictureChanged;
        public event UserRoleChanged userRoleChanged;

        public SkypeCredentials authInfo;
        public Tokens authTokens = new Tokens();
        public User selfProfile;
        public WebRequestFactory mainFactory;
        public LoginState authState = LoginState.Unknown;
        public SkypeTokenType tokenType = SkypeTokenType.MSNP24;
        public bool ignoreOwnEvents = true;

        private Poller mainPoll;
        private WebProxy mainProxy;
        private UserModule mainUserModule;
        private AuthModule mainAuthModule;
        private MessageModule mainMessageModule;
        private ContactModule mainContactModule;
        public Skype4Sharp(SkypeCredentials loginData, WebProxy loginProxy = null)
        {
            authInfo = loginData;
            mainProxy = loginProxy;
            mainFactory = new WebRequestFactory(mainProxy, new CookieContainer());
            mainPoll = new Poller(this);
            selfProfile = new User(this);
            mainUserModule = new UserModule(this);
            mainAuthModule = new AuthModule(this);
            mainMessageModule = new MessageModule(this);
            mainContactModule = new ContactModule(this);
        }
        public void StartPoll()
        {
            blockUnauthorized();
            mainPoll.StartPoll();
        }
        public void StopPoll()
        {
            blockUnauthorized();
            mainPoll.StopPoll();
        }
        public bool Login(bool bypassLogin = false)
        {
            if (!bypassLogin)
            {
                if (authState == LoginState.Success)
                {
                    throw new InvalidSkypeActionException("You are already signed in");
                }
                return mainAuthModule.Login();
            }
            else
            {
                mainPoll.StopPoll();
                bool loginSuccess = mainAuthModule.Login();
                mainPoll.StartPoll();
                return loginSuccess;
            }
        }
        public ChatMessage SendMessage(Chat targetChat, string newMessage, MessageType messageType = MessageType.Text)
        {
            blockUnauthorized();
            return mainMessageModule.createMessage(targetChat, newMessage, messageType);
        }
        public ChatMessage SendMessage(string targetUser, string newMessage, MessageType messageType = MessageType.Text)
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
            if (messageReceived == null) { return; }
            if (ignoreOwnEvents)
            {
                if (pMessage.Sender.Username == selfProfile.Username)
                {
                    return;
                }
            }
            messageReceived.Invoke(pMessage);
        }
        public void invokeContactReceived(User receivedUser, Chat originChat, User originUser)
        {
            if (contactReceived == null) { return; }
            if (ignoreOwnEvents)
            {
                if (originUser.Username == selfProfile.Username)
                {
                    return;
                }
            }
            contactReceived.Invoke(receivedUser, originChat, originUser);
        }
        public void invokeMessageEdited(ChatMessage pMessage)
        {
            if (messageEdited == null) { return; }
            if (ignoreOwnEvents)
            {
                if (pMessage.Sender.Username == selfProfile.Username)
                {
                    return;
                }
            }
            messageEdited.Invoke(pMessage);
        }
        public void invokeChatMembersChanged(Chat newChat, User eventInitiator, User eventTarget, ChatMemberChangedType changeType)
        {
            if (chatMembersChanged == null) { return; }
            chatMembersChanged.Invoke(newChat, eventInitiator, eventTarget, changeType);
        }
        public void invokeTopicChange(Chat targetChat, User eventInitiator, string newTopic)
        {
            if (topicChange == null) { return; }
            topicChange.Invoke(targetChat, eventInitiator, newTopic);
        }
        public void invokeContactRequestReceived(ContactRequest sentRequest)
        {
            if (contactRequestReceived == null) { return; }
            contactRequestReceived.Invoke(sentRequest);
        }
        public void invokeCallStarted(Chat originChat, User eventInitiator)
        {
            if (callStarted == null) { return; }
            callStarted.Invoke(originChat, eventInitiator);
        }
        public void invokeFileReceived(Events.SkypeFile sentFile)
        {
            if (fileReceived == null) { return; }
            fileReceived.Invoke(sentFile);
        }
        public void invokeChatPictureChanged(Chat targetChat, User eventInitiator, string newPicture)
        {
            if (chatPictureChanged == null) { return; }
            chatPictureChanged.Invoke(targetChat, eventInitiator, newPicture);
        }
        public void invokeUserRoleChanged(Chat newChat, User eventInitiator, User eventTarget, Enums.ChatRole newRole)
        {
            if (userRoleChanged == null) { return; }
            userRoleChanged.Invoke(newChat, eventInitiator, eventTarget, newRole);
        }
        private void blockUnauthorized()
        {
            if (authState != LoginState.Success)
            {
                throw new InvalidSkypeActionException("Not signed in");
            }
        }
    }
}