namespace Skype4Sharp.Auth
{
    public class SkypeCredentials
    {
        public string Username;
        public string Password;
        public SkypeCredentials(string authUser, string authPass)
        {
            Username = authUser;
            Password = authPass;
        }
    }
}
