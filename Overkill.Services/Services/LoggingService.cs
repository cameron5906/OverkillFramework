using Overkill.Services.Interfaces;
using Overkill.Services.Interfaces.Services;
using System;

namespace Overkill.Services.Services
{
    /// <summary>
    /// Responsible for logging and maintaining a "blackbox" log of vehicle diagnostics as well as cloud-based log reporting
    /// TODO: Cloud logging, file logging
    /// TODO: Actually use this class
    /// </summary>
    public class LoggingService : ILoggingService
    {
        public void Info(string info)
        {
            Console.WriteLine(info);
        }

        public void Error(Exception ex, string info)
        {
            Console.WriteLine(info);
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
