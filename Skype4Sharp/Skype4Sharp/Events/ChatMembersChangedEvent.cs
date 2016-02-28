namespace Skype4Sharp.Events
{
    public delegate void ChatMembersChanged(Chat newChat, User eventInitiator, User eventTarget, Enums.ChatMemberChangedType changeType);
}
