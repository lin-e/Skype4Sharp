namespace Skype4Sharp
{
    public class User
    {
        //add to this yourself. its very basic.
        public Enums.UserType Type;
        public string DisplayName;
        public string Username;
        public Skype4Sharp parentSkype;
        public User(Skype4Sharp skypeToUse)
        {
            parentSkype = skypeToUse;
        }
    }
}
