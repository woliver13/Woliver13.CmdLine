using System;

namespace Woliver13.CmdLine.Exceptions
{
    public class InvalidDefinitionException : ApplicationException
    {
        public InvalidDefinitionException(string message) : base(message)
        {
        }
    }
}
