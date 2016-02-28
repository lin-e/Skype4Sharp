# Skype4Sharp
Simple Web Skype implementation for C#
# Dependencies

- [Json.NET] (http://www.newtonsoft.com/json)
 
# Credits

- [Ghost] (https://github.com/NotGGhost/) Skidded so much code off him. And the MSPN24 token method
- [SpongyBacon] (https://github.com/sponges) Helped with any issues I was having, and moral support

# Events
- ChatMembersChanged
- ContactReceived
- ContactRequestReceived
- MessageEdited
- MessageReceived
- TopicChange
- 
# Usage
Look at the example bot for a working template, but if you really need full documentation, I'll provide it below.
The example is in C# Console, but it should be easy enough to adapt.

Logging in
```
static Skype4Sharp.Skype4Sharp mainSkype;
static SkypeCredentials authCreds = new SkypeCredentials("USERNAME", "PASSWORD");
static void Main(string[] args)
{
  mainSkype = new Skype4Sharp.Skype4Sharp(authCreds);
  mainSkype.Login();
}
```
Setting events
```
mainSkype.messageReceived += MainSkype_messageReceived;
mainSkype.contactRequestReceived += MainSkype_contactRequestReceived;
// Do the rest of the events yourself, these are the two most important ones in my opinion
mainSkype.StartPoll();
```
Accepting a contact
```
private static void MainSkype_contactRequestReceived(ContactRequest sentRequest)
{
  sentRequest.Accept();
}
```
Declining a contact
```
private static void MainSkype_contactRequestReceived(ContactRequest sentRequest)
{
  sentRequest.Decline();
}
```
Sending a group message, editing it and more
```
private static void MainSkype_messageReceived(ChatMessage pMessage)
{
  ChatMessage rMessage = pMessage.Chat.SendMessage("Processing your message...");
  rMessage.Body = "Second message";
  rMessage.Type = MessageType.RichText;
  rMessage.Body = "<b>THIS IS BOLD</b>";
}
```
Messaging a user
```
ChatMessage rMessage = mainSkype.SendMessage("c0mmodity", "Hello me!");
```
Adding or removing a user
```
mainSkype.AddUser("c0mmodity", "I'd like to add you on Skype!");
mainSkype.RemoveUser("c0mmodity");
```
Interacting with a chat (put in context, so it's easier for me to explain)
```
private static void MainSkype_messageReceived(ChatMessage pMessage)
{
  Chat newChat = pMessage.Chat;
  Console.WriteLine("The chat's topic is {0}", newChat.Topic);
  newChat.Topic = "Skype4Sharp";
  Console.WriteLine("My role in this chat is '{0}'", newChat.Role.ToString());
  newChat.Add("c0mmodity");
  newChat.SetAdmin("c0mmodity");
  Console.WriteLine("c0mmodity is a(n) {0}", newChat.UserRole("c0mmodity").ToString());
  newChat.Kick("eroded");
}
```
