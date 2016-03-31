# Skype4Sharp
Simple Web Skype implementation for C#.

By reading this project's source code, compiling as a binary, redistributing assets found in this repository, etc, or ANY form of use, you must agree to the license enlisted below.

If you do anything cool with this library, be sure to tell me :)

# License 
See: https://github.com/lin-e/Skype4Sharp/blob/master/LICENSE.md

# Official Forum Posts (others may be fake)
- [HackForums] (http://hackforums.net/showthread.php?tid=5187718)
- [LeakForums] (https://leakforums.net/thread-687213)

# Bots running this API
I don't endorse any of them, mainly cause they're just bad. (love you all)

- [SimpleSkype] (https://github.com/lin-e/SimpleSkype) (I feel like I have to add this, cause it's my own plugin-based bot running this API)
- [tafaBot] (http://hatscripts.com/addskype?tafabot.lnk)
- [illegalBot] (http://hatscripts.com/addskype?illegalpw)

# Events
- [x] ChatMembersChanged
- [x] ContactReceived
- [x] ContactRequestReceived
- [x] MessageEdited
- [x] MessageReceived
- [x] TopicChange
- [x] CallStarted
- [x] UserRoleChanged
- [x] GroupPictureChanged
- [x] FileReceived
- [ ] PictureReceived

# Credits
- [Ghost] (https://github.com/NotGGhost/) Skidded so much code off him. Also, the authentication code. 
> yung trump has permission to skid aids code to c#

- [SpongyBacon] (https://github.com/sponges) Helped with any issues I was having (a.k.a. "moral support")
- [Knackrack615] (http://knackrack615.me/) Helped with basic logic in the library

# Dependencies
- [Json.NET] (http://www.newtonsoft.com/json)

# Usage
Look at the example bot for a working template, but if you really need full documentation, I'll provide it below.
The example is in C# Console, but it should be easy enough to adapt.

Logging in
```C#
static Skype4Sharp.Skype4Sharp mainSkype;
static SkypeCredentials authCreds = new SkypeCredentials("USERNAME", "PASSWORD");
static void Main(string[] args)
{
  mainSkype = new Skype4Sharp.Skype4Sharp(authCreds);
  mainSkype.Login();
}
```
Setting events
```C#
mainSkype.messageReceived += MainSkype_messageReceived;
mainSkype.contactRequestReceived += MainSkype_contactRequestReceived;
// Do the rest of the events yourself, these are the two most important ones in my opinion
mainSkype.StartPoll();
```
Accepting a contact
```C#
private static void MainSkype_contactRequestReceived(ContactRequest sentRequest)
{
  sentRequest.Accept();
}
```
Declining a contact
```C#
private static void MainSkype_contactRequestReceived(ContactRequest sentRequest)
{
  sentRequest.Decline();
}
```
Sending a group message, editing it and more
```C#
private static void MainSkype_messageReceived(ChatMessage pMessage)
{
  ChatMessage rMessage = pMessage.Chat.SendMessage("Processing your message...");
  rMessage.Body = "Second message";
  rMessage.Type = MessageType.RichText;
  rMessage.Body = "<b>THIS IS BOLD</b>";
}
```
Messaging a user
```C#
ChatMessage rMessage = mainSkype.SendMessage("c0mmodity", "Hello me!");
```
Adding or removing a user
```C#
mainSkype.AddUser("c0mmodity", "I'd like to add you on Skype!");
mainSkype.RemoveUser("c0mmodity");
```
Interacting with a chat (put in context, so it's easier for me to explain)
```C#
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
  newChat.Leave();
}
```
Logging a call
```C#
private void MainSkype_callStarted(Chat originChat, User eventInitiator)
{
  Console.WriteLine("[EVENT]: CALL_STARTED > {0} ({1})", originChat.ID, eventInitiator.Username);
}
```

# Contact
If you have any issues, feel free to message me on Skype ('c0mmodity') or eMail me. My eMail is on my profile.

# FAQ
(Not actually questions I've been asked, just what I'd assume would be asked.)
---
Q : Why didn't you sell this?

A : It's not my code to sell. Also, as Skype4COM is bad enough as it is, you C# devs deserve options.

---
Q : This isn't working! I need help.

A : No. I don't offer formal support. If you really are stuck, message me on Skype (found in the Contacts section).

---
Q : I have a new feature I'd like for you to add.

A : Sure! Please raise it as an issue and I'll get back to you as soon as I can.

---
Q : Can I sell this code?

A : No. Read LICENSE.md if you want a more detailed answer, but you cannot sell my code as is, without any obvious modifications.

---
Q : *feature* is broken, what can I do?

A : Raise it as an issue, and I'll sort it out when I can. Please understand that I'm not paid for this, and I do have real world commitments.

---
Q : Wow! This is amazing, how can I help? (yeah, no-one's actually said this, but it'd be nice, y'know?)

A : You can donate to me via Bitcoin (1BKbYhqNVkKzZ5Q5p7Jtb2MyautnqBC9Qm), or just send me a nice message :smile:

---
