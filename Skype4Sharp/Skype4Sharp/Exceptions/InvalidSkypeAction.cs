using System;

namespace Skype4Sharp.Exceptions
{
    public class InvalidSkypeActionException : Exception
    {
        public InvalidSkypeActionException() { }
        public InvalidSkypeActionException(string message) : base(message) { }
        public InvalidSkypeActionException(string message, Exception inner) : base(message, inner) { }
    }
}