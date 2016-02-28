using System;

namespace Skype4Sharp.Exceptions
{
    public class InvalidSkypeParameterException : Exception
    {
        public InvalidSkypeParameterException() { }
        public InvalidSkypeParameterException(string message) : base(message) { }
        public InvalidSkypeParameterException(string message, Exception inner) : base(message, inner) { }
    }
}