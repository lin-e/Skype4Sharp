using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace Skype4Sharp.Skype4SharpCore
{
    class UserModule
    {
        private Skype4Sharp parentSkype;
        public UserModule(Skype4Sharp skypeToUse)
        {
            parentSkype = skypeToUse;
        }
        public User getUser(string inputName)
        {
            HttpWebRequest profileRequest = parentSkype.mainFactory.createWebRequest_POST("https://api.skype.com/users/batch/profiles", new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes("{\"usernames\":[\"" + inputName.JsonEscape() + "\"]}"), "application/json");
            string rawJSON = "";
            using (HttpWebResponse webResponse = (HttpWebResponse)profileRequest.GetResponse())
            {
                rawJSON = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
            }
            dynamic decodedJSON = JsonConvert.DeserializeObject(rawJSON);
            return userFromJson(decodedJSON[0]);
        }
        public User[] getUsers(string[] inputNames)
        {
            switch (inputNames.Length)
            {
                case 0:
                    throw new Exceptions.InvalidSkypeParameterException();
                case 1:
                    return new User[] { getUser(inputNames[0]) };
                default:
                    List<User> toReturn = new List<User>();
                    List<string> escapedNames = new List<string>();
                    foreach (string singleUser in inputNames)
                    {
                        escapedNames.Add(singleUser.JsonEscape());
                    }
                    string requestBody = "{\"usernames\":[\"" + string.Join("\",\"", escapedNames) + "\"]}";
                    HttpWebRequest profileRequest = parentSkype.mainFactory.createWebRequest_POST("https://api.skype.com/users/batch/profiles", new string[][] { new string[] { "X-Skypetoken", parentSkype.authTokens.SkypeToken } }, Encoding.ASCII.GetBytes(requestBody), "application/json");
                    string rawJSON = "";
                    using (HttpWebResponse webResponse = (HttpWebResponse)profileRequest.GetResponse())
                    {
                        rawJSON = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    }
                    dynamic decodedJSON = JsonConvert.DeserializeObject(rawJSON);
                    foreach (dynamic singleUserInfo in decodedJSON)
                    {
                        toReturn.Add(userFromJson(singleUserInfo));
                    }
                    return toReturn.ToArray();
            }
        }
        private User userFromJson(dynamic jsonObject)
        {
            User toReturn = new User(parentSkype);
            string firstName = jsonObject.firstname;
            string lastName = jsonObject.lastname;
            string userName = jsonObject.username;
            string finalName = "";
            if (firstName == null)
            {
                if (lastName == null)
                {
                    firstName = userName;
                }
                else
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
            toReturn.DisplayName = finalName;
            toReturn.Username = userName;
            toReturn.Type = (toReturn.Username.StartsWith("guest:")) ? Enums.UserType.Guest : Enums.UserType.Normal;
            return toReturn;
        }
        public User userFromContacts(dynamic jsonObject)
        {
            User toReturn = new User(parentSkype);
            toReturn.Username = (string)jsonObject.id;
            try
            {
                toReturn.DisplayName = (string)jsonObject.display_name;
            }
            catch
            {
                toReturn.DisplayName = toReturn.Username;
            }
            toReturn.Type = Enums.UserType.Normal;
            return toReturn;
        }
    }
}
