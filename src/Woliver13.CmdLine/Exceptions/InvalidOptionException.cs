using System;

namespace Woliver13.CmdLine.Exceptions
{
    public class InvalidOptionException : ApplicationException
    {            
        public InvalidOptionException(string message) : base(message)
        {
        }
    }
}
