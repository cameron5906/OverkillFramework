using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Core.Configuration
{
    public class SystemConfiguration
    {
        public string Module { get; set; }
        public string AuthorizationToken { get; set; }
        public string[] Plugins { get; set; }
    }
}
