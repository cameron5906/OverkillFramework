using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Common.Configuration
{
    public class InputConfiguration
    {
        public Dictionary<string, string> Keyboard { get; set; }
        public Dictionary<string, string> Gamepad { get; set; }
    }
}
