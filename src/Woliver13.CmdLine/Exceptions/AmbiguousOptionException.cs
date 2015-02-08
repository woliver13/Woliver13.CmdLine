using System;

namespace Woliver13.CmdLine.Exceptions
{
    public class AmbiguousOptionException : ApplicationException
    {
        public AmbiguousOptionException(string message) : base(message)
        {
        }
    }
}
