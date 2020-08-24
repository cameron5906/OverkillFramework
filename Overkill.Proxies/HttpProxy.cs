using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Overkill.Util.Helpers
{
    /// <summary>
    /// Proxy class to assist in unit testing HTTP related functionality
    /// </summary>
    public class HttpProxy : IHttpProxy
    {
        public void DownloadFile(string url, string localFile)
        {
            using (var webClient = new WebClient())
            {
                webClient.DownloadFileAsync(new Uri(url), localFile);
            }
        }

        public async Task<string> Get(string url)
        {
            using (var webClient = new WebClient())
            {
                return await webClient.DownloadStringTaskAsync(new Uri(url));
            }
        }

        public async Task<string> Post(string url, object payload)
        {
            using (var webClient = new WebClient())
            {
                var stringPayload = JsonSerializer.Serialize(payload);
                return await webClient.UploadStringTaskAsync(new Uri(url), stringPayload);
            }
        }
    }
}
