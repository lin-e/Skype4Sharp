using System.Net;
using System.Text;

namespace Skype4Sharp.Skype4SharpCore
{
    class MessageModule
    {
        private Skype4Sharp parentSkype;
        public MessageModule(Skype4Sharp skypeToUse)
        {
            parentSkype = skypeToUse;
        }
        public void editMessage(ChatMessage messageInfo, string newMessage)
        {
            HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_POST(messageInfo.Chat.ChatLink + "/messages", new string[][] { new string[] { "RegistrationToken", parentSkype.authTokens.RegistrationToken }, new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes("{\"content\":\"" + newMessage.JsonEscape() + "\",\"messagetype\":\"" + ((messageInfo.Type == Enums.MessageType.RichText) ? "RichText" : "Text") + "\",\"contenttype\":\"text\",\"skypeeditedid\":\"" + messageInfo.ID + "\"}"), "application/json");
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
        }
        public ChatMessage createMessage(Chat targetChat, string chatMessage, Enums.MessageType messageType)
        {
            ChatMessage toReturn = new ChatMessage(parentSkype);
            toReturn.Body = chatMessage;
            toReturn.Chat = targetChat;
            toReturn.Type = messageType;
            toReturn.ID = Helpers.Misc.getTime().ToString();
            toReturn.Sender = parentSkype.selfProfile;
            sendChatmessage(toReturn);
            return toReturn;
        }
        private void sendChatmessage(ChatMessage messageToSend)
        {
            HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_POST(messageToSend.Chat.ChatLink + "/messages", new string[][] { new string[] { "RegistrationToken", parentSkype.authTokens.RegistrationToken }, new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes("{\"content\":\"" + messageToSend.Body.JsonEscape() + "\",\"messagetype\":\"" + ((messageToSend.Type == Enums.MessageType.RichText) ? "RichText" : "Text") + "\",\"contenttype\":\"text\",\"clientmessageid\":\"" + messageToSend.ID + "\"}"), "application/json");
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
        }
    }
}
