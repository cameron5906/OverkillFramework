using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using Overkill.Proxies.Interfaces;
using Overkill.Services.Interfaces.Services;
using Overkill.Util;
using Overkill.Util.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Overkill.Services.Services
{
    /// <summary>
    /// Manages the playing of sound files (local and remote) as well as text to speech
    /// </summary>
    public class AudioService : IAudioService
    {
        private ISoundPlayerProxy soundPlayerProxy;
        private IFilesystemProxy filesystemProxy;
        private IHttpProxy httpProxy;

        public AudioService(ISoundPlayerProxy _soundPlayerProxy, IFilesystemProxy _filesystemProxy, IHttpProxy _httpProxy)
        {
            filesystemProxy = _filesystemProxy;
            httpProxy = _httpProxy;
            soundPlayerProxy = _soundPlayerProxy;
        }

        /// <summary>
        /// Download and play a sound file from a remote URL
        /// </summary>
        /// <param name="url">The direct URL to the sound file</param>
        public void PlayAudioFromURL(string url)
        {
            var extension = url.Substring(url.LastIndexOf(".") + 1);
            var localFileName = filesystemProxy.GenerateTempFilename(extension);
            httpProxy.DownloadFile(url, localFileName);

            PlayFromLocalFile(localFileName);
        }

        /// <summary>
        /// Play a local sound file
        /// </summary>
        /// <param name="localFile"></param>
        public void PlayFromLocalFile(string localFile)
        {
            soundPlayerProxy.Play(localFile);
        }

        /// <summary>
        /// Play Text2Speech
        /// </summary>
        /// <param name="text">The text to play from the speaker</param>
        public void SayText(string text)
        {
            throw new NotImplementedException();
        }
    }
}
