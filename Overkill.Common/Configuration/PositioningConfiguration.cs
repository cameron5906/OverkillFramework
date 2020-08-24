using Overkill.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Configuration
{
    public class PositioningConfiguration
    {
        public bool Enabled { get; set; }
        public PositioningSystem Type { get; set; }
        public string SerialInput { get; set; }
        public string SerialOutput { get; set; }
        public int SerialBaudRate { get; set; }
    }
}
