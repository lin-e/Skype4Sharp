using System.Net;
using System.Text;

namespace Skype4Sharp.Events
{
    public delegate void ContactRequestReceived(ContactRequest sentRequest);
    public class ContactRequest
    {
        private Skype4Sharp parentSkype;
        public User Sender;
        public string Body;
        public ContactRequest(Skype4Sharp skypeToUse)
        {
            parentSkype = skypeToUse;
        }
        public void Decline()
        {
            HttpWebRequest declineRequest = parentSkype.mainFactory.createWebRequest_PUT("https://api.skype.com/users/self/contacts/auth-request/" + Sender.Username + "/decline", new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes(""), "application/x-www-form-urlencoded");
            using (HttpWebResponse webResponse = (HttpWebResponse)declineRequest.GetResponse()) { }
        }
        public void Accept()
        {
            HttpWebRequest acceptRequest = parentSkype.mainFactory.createWebRequest_PUT("https://api.skype.com/users/self/contacts/auth-request/" + Sender.Username + "/accept", new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes(""), "application/x-www-form-urlencoded");
            using (HttpWebResponse webResponse = (HttpWebResponse)acceptRequest.GetResponse()) { }
        }
    }
}
