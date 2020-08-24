using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Util.Helpers
{
    /// <summary>
    /// Proxy class to assist in unit testing child process management functionality
    /// </summary>
    public class ProcessProxy : IProcessProxy
    {

        public async Task<(int ExitCode, string Output, string ErrorOutput)> ExecuteShellCommand(string command, params string[] args)
        {
            var tsc = new TaskCompletionSource<(int ExitCode, string Output, string ErrorOutput)>();
            
            var proc = new Process()
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = string.Join(" ", args.Select(arg => $"\"{arg}\"").ToArray()),
                    UseShellExecute = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };

            proc.BeginErrorReadLine();
            proc.BeginOutputReadLine();

            var outputLines = new List<string>();
            var errorLines = new List<string>();

            proc.OutputDataReceived += (sender, evt) => outputLines.Add(evt.Data.ToString());
            proc.ErrorDataReceived += (sender, evt) => errorLines.Add(evt.Data.ToString());
            proc.Exited += (sender, evt) =>
            {
                tsc.SetResult((
                    proc.ExitCode, 
                    string.Join(Environment.NewLine, outputLines), 
                    string.Join(Environment.NewLine, errorLines
                )));

                proc.Dispose();
            };

            return await tsc.Task;
        }
    }
}
