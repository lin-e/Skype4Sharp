using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace Skype4Sharp.Skype4SharpCore
{
    class ContactModule
    {
        private Skype4Sharp parentSkype;
        private UserModule userModule;
        public ContactModule(Skype4Sharp skypeToUse)
        {
            parentSkype = skypeToUse;
            userModule = new UserModule(parentSkype);
        }
        public User[] getContacts()
        {
            List<User> toReturn = new List<User>();
            HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_GET("https://contacts.skype.com/contacts/v1/users/" + parentSkype.selfProfile.Username + "/contacts?$filter=type%20eq%20%27skype%27%20or%20type%20eq%20%27msn%27%20or%20type%20eq%20%27pstn%27%20or%20type%20eq%20%27agent%27&reason=default", new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } });
            string rawInfo = "";
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                rawInfo = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            }
            dynamic jsonObject = JsonConvert.DeserializeObject(rawInfo);
            foreach (dynamic singleUser in jsonObject.contacts)
            {
                toReturn.Add(userModule.userFromContacts(singleUser));
            }
            return toReturn.ToArray();
        }
        public void addUser(string targetUsername, string requestMessage)
        {
            HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_PUT("https://api.skype.com/users/self/contacts/auth-request/" + targetUsername.ToLower(), new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes("greeting=" + requestMessage.UrlEncode()), "application/x-www-form-urlencoded");
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
        }
        public void deleteUser(string targetUsername)
        {
            HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_DELETE("https://contacts.skype.com/contacts/v1/users/" + parentSkype.selfProfile.Username + "/contacts/skype/" + targetUsername.ToLower(), new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } });
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse()) { }
        }
    }
}
