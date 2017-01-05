using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Skype4Sharp.Skype4SharpCore
{
    class AuthModule
    {
        private Skype4Sharp parentSkype;
        public AuthModule(Skype4Sharp skypeToUse)
        {
            parentSkype = skypeToUse;
        }
        public bool Login()
        {
            parentSkype.authState = Enums.LoginState.Processing;
            try
            {
                parentSkype.authTokens.SkypeToken = getSkypeToken();
                setRegTokenAndEndpoint();
                startSubscription();
                setProfile();
                parentSkype.authState = Enums.LoginState.Success;
                return true;
            }
            catch
            {
                parentSkype.authState = Enums.LoginState.Failed;
                return false;
            }
        }
        private string getSkypeToken()
        {
            switch (parentSkype.tokenType)
            {
                case Enums.SkypeTokenType.Standard:
                    HttpWebRequest standardTokenRequest = parentSkype.mainFactory.createWebRequest_GET("https://login.skype.com/login?client_id=578134&redirect_uri=https%3A%2F%2Fweb.skype.com", new string[][] { });
                    string uploadData = "";
                    using (HttpWebResponse webResponse = (HttpWebResponse)standardTokenRequest.GetResponse())
                    {
                        string rawDownload = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                        uploadData = string.Format("username={0}&password={1}&timezone_field={2}&js_time={3}&pie={4}&etm={5}&client_id=578134&redirect_uri={6}", parentSkype.authInfo.Username.UrlEncode(), parentSkype.authInfo.Password.UrlEncode(), DateTime.Now.ToString("zzz").Replace(":", "|").UrlEncode(), (Helpers.Misc.getTime() / 1000).ToString(), new Regex("<input type=\"hidden\" name=\"pie\" id=\"pie\" value=\"(.*?)\"/>").Match(rawDownload).Groups[1].ToString().UrlEncode(), new Regex("<input type=\"hidden\" name=\"etm\" id=\"etm\" value=\"(.*?)\"/>").Match(rawDownload).Groups[1].ToString().UrlEncode(), "https://web.skype.com".UrlEncode());
                    }
                    HttpWebRequest actualLogin = parentSkype.mainFactory.createWebRequest_POST("https://login.skype.com/login?client_id=578134&redirect_uri=https%3A%2F%2Fweb.skype.com", new string[][] { }, Encoding.ASCII.GetBytes(uploadData), "");
                    using (HttpWebResponse webResponse = (HttpWebResponse)actualLogin.GetResponse())
                    {
                        return new Regex("type=\"hidden\" name=\"skypetoken\" value=\"(.*?)\"").Match(new StreamReader(webResponse.GetResponseStream()).ReadToEnd()).Groups[1].ToString();
                    }
                case Enums.SkypeTokenType.MSNP24:
                    HttpWebRequest MSNP24TokenRequest = parentSkype.mainFactory.createWebRequest_POST("https://api.skype.com/login/skypetoken", new string[][] { }, Encoding.ASCII.GetBytes(string.Format("scopes=client&clientVersion=0/7.17.0.105//&username={0}&passwordHash={1}", parentSkype.authInfo.Username, Convert.ToBase64String(Helpers.Misc.hashMD5_Byte(string.Format("{0}\nskyper\n{1}", parentSkype.authInfo.Username, parentSkype.authInfo.Password))))), "");
                    string rawJSON = "";
                    using (HttpWebResponse webResponse = (HttpWebResponse)MSNP24TokenRequest.GetResponse())
                    {
                        rawJSON = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    }
                    dynamic decodedJSON = JsonConvert.DeserializeObject(rawJSON);
                    return decodedJSON ? decodedJSON.skypetoken : null;
                default:
                    return null;
            }
        }
        private void setRegTokenAndEndpoint()
        {
            HttpWebRequest webRequest = parentSkype.mainFactory.createWebRequest_POST("https://client-s.gateway.messenger.live.com/v1/users/ME/endpoints", new string[][] { new string[] { "Authentication", "skypetoken=" + parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes("{}"), "application/x-www-form-urlencoded");
            using (HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse())
            {
                parentSkype.authTokens.RegistrationToken = webResponse.GetResponseHeader("Set-RegistrationToken").Split(';')[0];
                parentSkype.authTokens.Endpoint = webResponse.GetResponseHeader("Location");
                parentSkype.authTokens.EndpointID = webResponse.GetResponseHeader("Set-RegistrationToken").Split(';')[2].Split('=')[1];
            }
        }
        private void startSubscription()
        {
            HttpWebRequest propertiesRequest = parentSkype.mainFactory.createWebRequest_PUT("https://client-s.gateway.messenger.live.com/v1/users/ME/endpoints/SELF/properties?name=supportsMessageProperties", new string[][] { new string[] { "RegistrationToken", parentSkype.authTokens.RegistrationToken } }, Encoding.ASCII.GetBytes("{\"supportsMessageProperties\":true}"), "application/json");
            using (HttpWebResponse webResponse = (HttpWebResponse)propertiesRequest.GetResponse()) { }
            HttpWebRequest subscriptionRequest = parentSkype.mainFactory.createWebRequest_POST("https://client-s.gateway.messenger.live.com/v1/users/ME/endpoints/SELF/subscriptions", new string[][] { new string[] { "RegistrationToken", parentSkype.authTokens.RegistrationToken } }, Encoding.ASCII.GetBytes("{\"channelType\":\"httpLongPoll\",\"template\":\"raw\",\"interestedResources\":[\"/v1/users/ME/conversations/ALL/properties\",\"/v1/users/ME/conversations/ALL/messages\",\"/v1/users/ME/contacts/ALL\",\"/v1/threads/ALL\"]}"), "application/json");
            using (HttpWebResponse webResponse = (HttpWebResponse)subscriptionRequest.GetResponse()) { }
        }
        private void setProfile()
        {
            HttpWebRequest selfProfileRequest = parentSkype.mainFactory.createWebRequest_GET("https://api.skype.com/users/self/profile", new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } });
            string rawJSON = "";
            using (HttpWebResponse webResponse = (HttpWebResponse)selfProfileRequest.GetResponse())
            {
                rawJSON = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            }
            dynamic decodedJSON = JsonConvert.DeserializeObject(rawJSON);
            string firstName = decodedJSON.firstname;
            string lastName = decodedJSON.lastname;
            string userName = decodedJSON.username;
            string finalName = "";
            if (firstName == null)
            {
                if (lastName != null)
                {
                    finalName = lastName;
                }
            }
            else
            {
                if (lastName == null)
                {
                    finalName = firstName;
                }
                else
                {
                    finalName = firstName + " " + lastName;
                }
            }
            parentSkype.selfProfile.DisplayName = finalName;
            parentSkype.selfProfile.Username = userName;
            parentSkype.selfProfile.Type = Enums.UserType.Normal;
        }
    }
}
