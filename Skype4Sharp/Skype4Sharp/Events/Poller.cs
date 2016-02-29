using System.Threading;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace Skype4Sharp.Events
{
    class Poller
    {
        private List<string> processedContactRequests;
        Skype4Sharp parentSkype;
        public Poller(Skype4Sharp toUse)
        {
            parentSkype = toUse;
            processedContactRequests = new List<string>();
        }
        public void StartPoll()
        {
            new Thread(() => // Main Poll
            {
                while (true)
                {
                    string rawInfo = "";
                    HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_POST("https://client-s.gateway.messenger.live.com/v1/users/ME/endpoints/SELF/subscriptions/0/poll", new string[][] { new string[] { "RegistrationToken", parentSkype.authTokens.RegistrationToken } }, Encoding.ASCII.GetBytes(""), "application/json");
                    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        rawInfo = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    }
                    ProcessPoll(rawInfo);
                }
            }).Start();
            new Thread(() => // Contact Requests
            {
                while (true)
                {
                    HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_GET("https://api.skype.com/users/self/contacts/auth-request", new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } });
                    string rawInfo = "";
                    using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
                    {
                        rawInfo = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    }
                    dynamic allData = JsonConvert.DeserializeObject(rawInfo);
                    foreach (dynamic singleRequest in allData)
                    {
                        string senderName = (string)singleRequest.sender;
                        if (!(processedContactRequests.Contains(senderName)))
                        {
                            processedContactRequests.Add(senderName);
                            User requestSender = parentSkype.GetUser(senderName);
                            ContactRequest newRequest = new ContactRequest(parentSkype);
                            newRequest.Sender = requestSender;
                            newRequest.Body = ((string)singleRequest.greeting).HtmlDecode();
                            parentSkype.invokeContactRequestReceived(newRequest);
                        }
                    }
                    Thread.Sleep(10000);
                }
            }).Start();
        }
        public void ProcessPoll(string rawInfo)
        {
            string[] returnIf = { "{}", null, "" };
            if (!(returnIf.Contains(rawInfo)))
            {
                dynamic allData = JsonConvert.DeserializeObject(rawInfo);
                foreach (dynamic singleMessage in allData.eventMessages)
                {
                    string messageType = (string)(singleMessage.resource.messagetype);
                    switch (messageType)
                    {
                        case "ThreadActivity/DeleteMember":
                        case "ThreadActivity/AddMember":
                        case "ThreadActivity/TopicUpdate":
                        case "ThreadActivity/PictureUpdate":
                        case "ThreadActivity/RoleUpdate":
                            {
                                Chat messageChat = new Chat(parentSkype);
                                messageChat.ChatLink = (string)singleMessage.resource.conversationLink;
                                string[] chatLinkItems = messageChat.ChatLink.Split('/');
                                messageChat.ID = chatLinkItems[chatLinkItems.Length - 1];
                                messageChat.Type = (messageChat.ID.StartsWith("8:") ? Enums.ChatType.Private : Enums.ChatType.Group);
                                switch (messageType)
                                {
                                    case "ThreadActivity/AddMember":
                                        {
                                            Regex userFinder = new Regex("<addmember><eventtime>(.*?)</eventtime><initiator>8:(.*?)</initiator><target>8:(.*?)</target></addmember>");
                                            User eventInitiator = parentSkype.GetUser(userFinder.Match(((string)singleMessage.resource.content)).Groups[2].ToString());
                                            User eventTarget = parentSkype.GetUser(userFinder.Match(((string)singleMessage.resource.content)).Groups[3].ToString());
                                            parentSkype.invokeChatMembersChanged(messageChat, eventInitiator, eventTarget, (eventInitiator.Username == eventTarget.Username) ? Enums.ChatMemberChangedType.Joined : Enums.ChatMemberChangedType.Added);
                                        }
                                        break;
                                    case "ThreadActivity/DeleteMember":
                                        {
                                            Regex userFinder = new Regex("<deletemember><eventtime>(.*?)</eventtime><initiator>8:(.*?)</initiator><target>8:(.*?)</target></deletemember>");
                                            User eventInitiator = parentSkype.GetUser(userFinder.Match(((string)singleMessage.resource.content)).Groups[2].ToString());
                                            User eventTarget = parentSkype.GetUser(userFinder.Match(((string)singleMessage.resource.content)).Groups[3].ToString());
                                            parentSkype.invokeChatMembersChanged(messageChat, eventInitiator, eventTarget, (eventInitiator.Username == eventTarget.Username) ? Enums.ChatMemberChangedType.Left : Enums.ChatMemberChangedType.Removed);
                                        }
                                        break;
                                    case "ThreadActivity/TopicUpdate":
                                        {
                                            Regex topicFinder = new Regex("<topicupdate><eventtime>(.*?)</eventtime><initiator>8:(.*?)</initiator><value>(.*?)</value></topicupdate>");
                                            User eventInitiator = parentSkype.GetUser(topicFinder.Match(((string)singleMessage.resource.content)).Groups[2].ToString());
                                            string newTopic = topicFinder.Match(((string)singleMessage.resource.content)).Groups[3].ToString();
                                            newTopic = newTopic.HtmlDecode();
                                            parentSkype.invokeTopicChange(messageChat, eventInitiator, newTopic.HtmlDecode());
                                        }
                                        break;
                                    case "ThreadActivity/PictureUpdate":
                                        {
                                            Regex pictureFinder = new Regex("<pictureupdate><eventtime>(.*?)</eventtime><initiator>8:(.*?)</initiator><value>(.*?)</value></pictureupdate>");
                                            User eventInitiator = parentSkype.GetUser(pictureFinder.Match(((string)singleMessage.resource.content)).Groups[2].ToString());
                                            string imageURL = pictureFinder.Match(((string)singleMessage.resource.content)).Groups[3].ToString();
                                            parentSkype.invokeChatPictureChanged(messageChat, eventInitiator, imageURL.Remove(0, 4));
                                        }
                                        break;
                                    case "ThreadActivity/RoleUpdate":
                                        {
                                            Regex roleFinder = new Regex("<roleupdate><eventtime>(.*?)</eventtime><initiator>8:(.*?)</initiator><target><id>8:(.*?)</id><role>(.*?)</role></target></roleupdate>");
                                            User eventInitiator = parentSkype.GetUser(roleFinder.Match(((string)singleMessage.resource.content)).Groups[2].ToString());
                                            User eventTarget = parentSkype.GetUser(roleFinder.Match(((string)singleMessage.resource.content)).Groups[3].ToString());
                                            string newRoleString = roleFinder.Match(((string)singleMessage.resource.content)).Groups[4].ToString();
                                            Enums.ChatRole newRole = (newRoleString == "user") ? Enums.ChatRole.User : Enums.ChatRole.Admin;
                                            parentSkype.invokeUserRoleChanged(messageChat, eventInitiator, eventTarget, newRole);
                                        }
                                        break;
                                }
                            }
                            break;
                        case "Control/Typing": //the following events all have senders
                        case "Control/ClearTyping":
                        case "Text":
                        case "RichText":
                        case "RichText/Contacts":
                        case "Event/Call":
                        case "RichText/Files":
                            {
                                Chat messageChat = new Chat(parentSkype);
                                messageChat.ChatLink = (string)singleMessage.resource.conversationLink;
                                string[] chatLinkItems = messageChat.ChatLink.Split('/');
                                messageChat.ID = chatLinkItems[chatLinkItems.Length - 1];
                                messageChat.Type = (messageChat.ID.StartsWith("8:") ? Enums.ChatType.Private : Enums.ChatType.Group);
                                User messageSender = new User(parentSkype);
                                messageSender.Username = ((string)singleMessage.resource.from).Split(new string[] { "8:" }, StringSplitOptions.None)[1];
                                messageSender.DisplayName = (string)singleMessage.resource.imdisplayname;
                                messageSender.Type = (messageSender.Username.StartsWith("guest:") ? Enums.UserType.Guest : Enums.UserType.Normal);
                                switch (messageType)
                                {
                                    case "Control/Typing": // Event fired when a user starts typing. You can add an event for this if you want.
                                    case "Control/ClearTyping": //Event fired when user clears their text?
                                        break;
                                    case "Text":
                                    case "RichText":
                                        {
                                            ChatMessage newMessage = new ChatMessage(parentSkype);
                                            newMessage.Sender = messageSender;
                                            newMessage.Chat = messageChat;
                                            newMessage.ID = (string)singleMessage.resource.id;
                                            newMessage.Type = (messageType == "Text") ? Enums.MessageType.Text : Enums.MessageType.RichText;
                                            if (singleMessage.resource.skypeeditedid == null)
                                            {
                                                string newMessageBody = ((string)singleMessage.resource.content);
                                                if (newMessage.Type == Enums.MessageType.RichText)
                                                {
                                                    newMessageBody = newMessageBody.StripTags();
                                                }
                                                newMessage.Body = newMessageBody.HtmlDecode();
                                                parentSkype.invokeMessageReceived(newMessage);
                                            }
                                            else
                                            {
                                                Regex toCheck = new Regex("Edited previous message: (.*?)<e_m ts=\"(.*?)\" a=\"(.*?)\" t=\"(.*?)\"/>");
                                                string decodedMessage = ((string)singleMessage.resource.content).HtmlDecode();
                                                try
                                                {
                                                    string newMessageBody = (toCheck.IsMatch(decodedMessage)) ? toCheck.Match(decodedMessage).Groups[1].ToString() : decodedMessage;
                                                    if (newMessage.Type == Enums.MessageType.RichText)
                                                    {
                                                        newMessageBody = newMessageBody.StripTags();
                                                    }
                                                    newMessage.Body = newMessageBody.HtmlDecode();
                                                }
                                                catch
                                                {
                                                    newMessage.Body = "";
                                                }
                                                parentSkype.invokeMessageEdited(newMessage);
                                            }
                                        }
                                        break;
                                    case "RichText/Contacts":
                                        {
                                            User sentContact = new User(parentSkype);
                                            string rawContactData = (string)singleMessage.resource.content;
                                            sentContact.DisplayName = new Regex("f=\"(.*?)\"").Match(rawContactData).Groups[1].ToString();
                                            string contactUsername = new Regex("s=\"(.*?)\"").Match(rawContactData).Groups[1].ToString();
                                            sentContact.Username = (contactUsername.Length == 0) ? sentContact.DisplayName : contactUsername;
                                            sentContact.Type = Enums.UserType.Normal; // I doubt you can send a guest as a contact.
                                            parentSkype.invokeContactReceived(sentContact, messageChat, messageSender);
                                        }
                                        break;
                                    case "Event/Call":
                                        {
                                            parentSkype.invokeCallStarted(messageChat, messageSender);
                                        }
                                        break;
                                    case "RichText/Files":
                                        {
                                            string rawContents = (string)singleMessage.resource.content;
                                            Regex fileInfo = new Regex("<file size=\"(.*?)\" index=\"(.*?)\" tid=\"(.*?)\">(.*?)</file>");
                                            foreach (Match singleMatch in fileInfo.Matches(rawContents))
                                            {
                                                SkypeFile sentFile = new SkypeFile(parentSkype);
                                                sentFile.Sender = messageSender;
                                                sentFile.Chat = messageChat;
                                                sentFile.Name = singleMatch.Groups[4].ToString();
                                                sentFile.Size = Convert.ToInt32(singleMatch.Groups[1].ToString());
                                                parentSkype.invokeFileReceived(sentFile);
                                            }
                                        }
                                        break;
                                }
                            }
                            break;
                    }
                }
            }
        }
    }
}
