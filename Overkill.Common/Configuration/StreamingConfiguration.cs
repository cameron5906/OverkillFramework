using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Configuration
{
    public class StreamingConfiguration
    {
        public bool Enabled { get; set; }
        public string FFmpegExecutablePath { get; set; }
        public string Endpoint { get; set; }
        public string[] Devices { get; set; }
    }
}
