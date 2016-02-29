namespace Skype4Sharp.Events
{
    public delegate void FileReceived(SkypeFile sentFile);
    public class SkypeFile
    {
        private Skype4Sharp parentSkype;
        public User Sender;
        public Chat Chat;
        public int Size;
        public string Name;
        public SkypeFile(Skype4Sharp skypeToUse)
        {
            parentSkype = skypeToUse;
        }
    }
}
