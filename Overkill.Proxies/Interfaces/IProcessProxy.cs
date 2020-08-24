using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Proxies.Interfaces
{
    public interface IProcessProxy
    {
        Task<(int ExitCode, string Output, string ErrorOutput)> ExecuteShellCommand(string command, params string[] args);
    }
}
