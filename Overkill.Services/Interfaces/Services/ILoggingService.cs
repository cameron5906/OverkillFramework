using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Services.Interfaces.Services
{
    public interface ILoggingService
    {
        void Info(string info);
        void Error(Exception ex, string info);
    }
}
