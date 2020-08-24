using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Overkill.Util.Helpers
{
    /// <summary>
    /// Proxy class to support unit testing of Filesystem I/O
    /// </summary>
    public class FilesystemProxy : IFilesystemProxy
    {
        public string GenerateTempFilename(string extension)
        {
            return $"{Path.GetTempPath()}{Guid.NewGuid()}.{extension}";
        }

        public void WriteFile(string fileName, string data)
        {
            File.WriteAllText(fileName, data);
        }

        public string ReadFile(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public void DeleteFile(string fileName)
        {
            File.Delete(fileName);
        }
    }
}
