using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Common.Exceptions
{
    public class BootException : Exception
    {
        public BootException(string message) : base(message)
        {
        }

        public BootException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
