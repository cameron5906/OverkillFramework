using CSCore.Codecs;
using CSCore.SoundOut;
using Moq;
using Overkill.Proxies.Interfaces;
using Overkill.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Overkill.Services.Tests
{
    public class AudioServiceTestFixture
    {
        AudioService audioService;
        Mock<IHttpProxy> httpProxy;
        Mock<IFilesystemProxy> filesystemProxy;
        Mock<ISoundPlayerProxy> soundPlayerProxy;


        public AudioServiceTestFixture()
        {
            httpProxy = new Mock<IHttpProxy>();
            filesystemProxy = new Mock<IFilesystemProxy>();
            soundPlayerProxy = new Mock<ISoundPlayerProxy>();

            audioService = new AudioService(soundPlayerProxy.Object, filesystemProxy.Object, httpProxy.Object);

        }

        [Fact]
        public void PlayAudioFromURL_GeneratesTemporaryFilename_WithCorrectExtension()
        {
            filesystemProxy.Setup(x => x.GenerateTempFilename(It.IsAny<string>())).Returns(() => "test.mp3");
            audioService.PlayAudioFromURL("http://mysound.com/test.mp3");
            filesystemProxy.Verify(x => x.GenerateTempFilename("mp3"));
        }

        [Fact]
        public void PlayAudioFromURL_DownloadsFileFromURL()
        {
            audioService.PlayAudioFromURL("http://mysound.com/test.mp3");
            httpProxy.Verify(x => x.DownloadFile("http://mysound.com/test.mp3", It.IsAny<string>()));
        }

        [Fact]
        public void PlayAudioFromURL_PlaysFile_WithCorrectFilename()
        {
            filesystemProxy.Setup(x => x.GenerateTempFilename(It.IsAny<string>())).Returns("test.mp3");
            audioService.PlayAudioFromURL("http://mysound.com/test.mp3");
            soundPlayerProxy.Verify(x => x.Play("test.mp3"));
        }

        [Fact]
        public void PlayFromLocalFile_PlaysFile_WithCorrectFilename()
        {
            audioService.PlayFromLocalFile("myaudio.mp3");
            soundPlayerProxy.Verify(x => x.Play("myaudio.mp3"));
        }
    }
}
