using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Common.Exceptions
{
    public class InputAlreadyBoundException : Exception
    {
        public InputAlreadyBoundException(string name) : base($"Duplicate input binding detected: {name}")
        {

        }
    }
}
