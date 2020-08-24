using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Overkill.Proxies.Interfaces
{
    public interface IHttpProxy
    {
        void DownloadFile(string url, string localFilePath);
        Task<string> Get(string url);
        Task<string> Post(string url, object data);
    }
}
