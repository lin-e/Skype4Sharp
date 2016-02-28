using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skype4Sharp.Helpers
{
    public static class Extensions
    {
        public static ChatMessage SendMessage(this Chat targetChat, string newMessage, Enums.MessageType messageType = Enums.MessageType.Text)
        {
            return targetChat.parentSkype.SendMessage(targetChat, newMessage, messageType);
        }
        public static ChatMessage SendMessage(this User targetUser, string newMessage, Enums.MessageType messageType = Enums.MessageType.Text)
        {
            return targetUser.parentSkype.SendMessage(targetUser.Username, newMessage, messageType);
        }
        public static void Add(this Chat targetChat, User targetUser)
        {
            targetChat.Add(targetUser.Username);
        }
        public static void Kick(this Chat targetChat, User targetUser)
        {
            targetChat.Kick(targetUser.Username);
        }
        public static void Promote(this Chat targetChat, User targetUser)
        {
            targetChat.SetAdmin(targetUser.Username);
        }
        public static void Add(this User targetUser, string requestMessage)
        {
            targetUser.parentSkype.AddUser(targetUser.Username, requestMessage);
        }
        public static void Remove(this User targetUser)
        {
            targetUser.parentSkype.RemoveUser(targetUser.Username);
        }
    }
}
