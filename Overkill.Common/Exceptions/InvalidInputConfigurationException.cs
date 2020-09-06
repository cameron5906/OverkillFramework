using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Common.Exceptions
{
    public class InvalidInputConfigurationException : Exception
    {
        public InvalidInputConfigurationException(string inputName) : base($"Invalid value for input: {inputName}. Value must be a valid keyboard key, 'joystick', or 'trigger'.")
        {

        }
    }
}
