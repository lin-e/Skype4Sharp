using System;
using System.Collections.Generic;
using System.Linq;
using Skype4Sharp;
using Skype4Sharp.Events;
using Skype4Sharp.Auth;
using Skype4Sharp.Helpers;
using Skype4Sharp.Enums;
using System.Threading;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace ExampleBot
{
    class Program
    {
        static Skype4Sharp.Skype4Sharp mainSkype;
        static SkypeCredentials authCreds = new SkypeCredentials("USERNAME", "PASSWORD");
        static string triggerString = "!";
        static void Main(string[] args)
        {
            mainSkype = new Skype4Sharp.Skype4Sharp(authCreds);
            Console.WriteLine("[DEBUG]: Logging in with {0}:{1}", authCreds.Username, string.Join("", Enumerable.Repeat("*", authCreds.Password.Length)));
            mainSkype.Login();
            Console.WriteLine("[DEBUG]: Login complete");
            mainSkype.messageReceived += MainSkype_messageReceived;
            mainSkype.contactRequestReceived += MainSkype_contactRequestReceived;
            Console.WriteLine("[DEBUG]: Events set");
            mainSkype.StartPoll();
            Console.WriteLine("[DEBUG]: Poll started");
            while (true) { }
        }

        private static void MainSkype_contactRequestReceived(ContactRequest sentRequest)
        {
            new Thread(() =>
            {
                Console.WriteLine("[EVENT]: REQUEST_RECEIVED > {0}: {1}", sentRequest.Sender.Username, sentRequest.Body);
                sentRequest.Accept();
                sentRequest.Sender.SendMessage("Thanks for adding me!");
            }).Start();
        }

        private static void MainSkype_messageReceived(ChatMessage pMessage)
        {
            new Thread(() =>
            {
                try
                {
                    Console.WriteLine("[EVENT]: MESSAGE_RECEIVED > {0} ({2}): {1}", pMessage.Sender.Username, pMessage.Body, pMessage.Chat.ID);
                    string[] commandArgs = pMessage.Body.Split(' ');
                    if (commandArgs[0].ToLower().StartsWith(triggerString))
                    {
                        ChatMessage rMessage = pMessage.Chat.SendMessage("Processing your command...");
                        WebClient webClient = new WebClient();
                        rMessage.Type = MessageType.RichText;
                        bool apiError = false;
                        switch (commandArgs[0].Remove(0, triggerString.Length).ToLower())
                        {
                            case "help":
                                {
                                    string commandPrefix = Environment.NewLine + "    " + triggerString;
                                    rMessage.Body = "This is a testing bot by <a href=\"https://twitter.com/c0mmodity\">Commodity</a> - It serves no actual purpose, much like the rest of my projects" +
                                    commandPrefix + "ping - wow such an original fucking command" +
                                    commandPrefix + "currency - idk shows you currency info or something" +
                                    commandPrefix + "btcwallet - generates a bitcoin wallet keypair" + 
                                    commandPrefix + "numberfact <number> [math|trivia|year] - idk number facts or something".HtmlEncode() +
                                    commandPrefix + "visualdns <URL> - visualises a domain's dns. (no shit)".HtmlEncode();
                                }
                                break;
                            case "ping":
                                {
                                    rMessage.Body = "<b>@" + pMessage.Sender.Username + " (" + pMessage.Sender.DisplayName + ")</b> Pong!";
                                }
                                break;
                            case "currency":
                                if (true)
                                {
                                    rMessage.Body = "Getting current exchange rates...";
                                    try
                                    {
                                        rMessage.Body = "All rates are in terms of USD (e.g. 1 USD is worth X GBP) :" + Environment.NewLine
                                            + "<b>GBP: </b>" + webClient.DownloadString("http://www.freecurrencyconverterapi.com/api/v3/convert?q=USD_GBP&compact=y").Replace("{\"USD_GBP\":{\"val\":", "").Replace("}}", "") + Environment.NewLine
                                            + "<b>BTC: </b>" + webClient.DownloadString("http://www.freecurrencyconverterapi.com/api/v3/convert?q=USD_BTC&compact=y").Replace("{\"USD_BTC\":{\"val\":", "").Replace("}}", "") + Environment.NewLine
                                            + "<b>EUR: </b>" + webClient.DownloadString("http://www.freecurrencyconverterapi.com/api/v3/convert?q=USD_EUR&compact=y").Replace("{\"USD_EUR\":{\"val\":", "").Replace("}}", "") + Environment.NewLine
                                            + "<b>AUD: </b>" + webClient.DownloadString("http://www.freecurrencyconverterapi.com/api/v3/convert?q=USD_AUD&compact=y").Replace("{\"USD_AUD\":{\"val\":", "").Replace("}}", "") + Environment.NewLine
                                            + "<b>JPY: </b>" + webClient.DownloadString("http://www.freecurrencyconverterapi.com/api/v3/convert?q=USD_JPY&compact=y").Replace("{\"USD_JPY\":{\"val\":", "").Replace("}}", "") + Environment.NewLine
                                            + "<b>CAD: </b>" + webClient.DownloadString("http://www.freecurrencyconverterapi.com/api/v3/convert?q=USD_CAD&compact=y").Replace("{\"USD_CAD\":{\"val\":", "").Replace("}}", "") + Environment.NewLine
                                            + "<b>CHF: </b>" + webClient.DownloadString("http://www.freecurrencyconverterapi.com/api/v3/convert?q=USD_CHF&compact=y").Replace("{\"USD_CHF\":{\"val\":", "").Replace("}}", "") + Environment.NewLine;
                                    }
                                    catch
                                    {
                                        apiError = true;
                                    }
                                }
                                break;
                            case "btcwallet":
                                if (true)
                                {
                                    rMessage.Body = "Generating a Bitcoin keypair...";
                                    try
                                    {
                                        string rawDownload = webClient.DownloadString("https://blockchain.info/q/newkey");
                                        string[] keypairParts = rawDownload.Split(' ');
                                        rMessage.Body = "<b>Public key: </b>" + keypairParts[0] + Environment.NewLine
                                            + "<b>Private key: </b>" + keypairParts[1];
                                    }
                                    catch
                                    {
                                        apiError = true;
                                    }
                                }
                                break;
                            case "numberfact":
                                if (commandArgs.Length > 1)
                                {
                                    rMessage.Body = "Finding a fact about " + commandArgs[1] + "...";
                                    try
                                    {
                                        int testConvert = Convert.ToInt16(commandArgs[1]);
                                        string mathArgument = "trivia";
                                        if (commandArgs.Length > 2)
                                        {
                                            if ((commandArgs[2].ToLower() == "math") || (commandArgs[2].ToLower() == "year"))
                                            {
                                                mathArgument = commandArgs[2].ToLower();
                                            }
                                        }
                                        try
                                        {
                                            rMessage.Body = "<b>" + commandArgs[1] + ":</b> " + webClient.DownloadString("http://numbersapi.com/" + commandArgs[1] + "/" + mathArgument);
                                        }
                                        catch
                                        {
                                            apiError = true;
                                        }
                                    }
                                    catch
                                    {
                                        rMessage.Body = "'" + commandArgs[1] + "' is not a valid value for an integer.";
                                    }
                                }
                                else
                                {
                                    rMessage.Body = "Syntax error. The correct syntax for this command is " + triggerString + "numberfact <number> [math|trivia|year]".HtmlEncode();
                                }
                                break;
                            case "visualdns":
                                if (true)
                                {
                                    if (commandArgs.Length == 2)
                                    {
                                        rMessage.Body = "Generating a visualization of " + commandArgs[1] + "'s DNS servers...";
                                        try
                                        {
                                            string rawDownload = toImgur("http://www.inmotionhosting.com/support/modules/mod_visual_dig/visual_dig.php?domain=" + commandArgs[1]);
                                            if (rawDownload.EndsWith(".gif"))
                                            {
                                                rawDownload = "There has been an error generating an image. You have either:" + Environment.NewLine
                                                    + " - entered an invalid domain" + Environment.NewLine
                                                    + " - The DNS servers aren't set up properly" + Environment.NewLine
                                                    + " - You have entered a subdomain (like www.) or you added 'http://'";
                                                rMessage.Body = rawDownload;
                                            }
                                            else
                                            {
                                                rMessage.Body = "<b>" + commandArgs[1] + ": </b>[<a href=\"" + rawDownload + "\">CLICK HERE</a>]";
                                            }
                                        }
                                        catch
                                        {
                                            apiError = true;
                                        }
                                    }
                                    else
                                    {
                                        rMessage.Body = "Syntax error. The correct syntax for this command is " + triggerString + "visualdns <URL>".HtmlEncode();
                                    }
                                }
                                break;
                            default:
                                {
                                    rMessage.Body = "Invalid command. Please use " + triggerString + "help for a list of commands.";
                                }
                                break;
                        }
                        if (apiError)
                        {
                            rMessage.Body = "<font color=\"#b30000\"><b>Error in calling API</b></font>";
                        }
                    }
                } catch { }
            }).Start();
        }
        public static string toImgur(string rawURL)
        {
            string imgurURL = "";
            WebClient w = new WebClient();
            w.Headers.Add("Authorization", "Client-ID IMGUR_KEY");
            System.Collections.Specialized.NameValueCollection Keys = new System.Collections.Specialized.NameValueCollection();
            Keys.Add("image", Convert.ToBase64String(w.DownloadData(rawURL)));
            byte[] responseArray = w.UploadValues("https://api.imgur.com/3/image", Keys);
            dynamic result = Encoding.ASCII.GetString(responseArray);
            Regex regEx = new Regex("link\":\"(.*?)\"");
            Match match = regEx.Match(result);
            string url = match.ToString().Replace("link\":\"", "").Replace("\"", "").Replace("\\/", "/");
            imgurURL = url;
            return imgurURL;
        }
    }
}
