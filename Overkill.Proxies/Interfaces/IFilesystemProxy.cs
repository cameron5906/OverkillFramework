using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Proxies.Interfaces
{
    public interface IFilesystemProxy
    {
        string GenerateTempFilename(string extension);
        void WriteFile(string filePath, string data);
        string ReadFile(string filePath);
        void DeleteFile(string filePath);
    }
}
