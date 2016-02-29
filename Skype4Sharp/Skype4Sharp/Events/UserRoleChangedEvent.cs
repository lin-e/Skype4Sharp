namespace Skype4Sharp.Events
{
    public delegate void UserRoleChanged(Chat newChat, User eventInitiator, User eventTarget, Enums.ChatRole newRole);
}
