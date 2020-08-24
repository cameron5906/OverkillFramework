using Microsoft.Extensions.Configuration;
using Overkill.Core.Interfaces;
using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xabe.FFmpeg;

namespace Overkill.Core
{
    /// <summary>
    /// Video transmission service which uses FFmpeg to pipe data coming from a capture device to a remote Websocket server using MPEG1
    /// </summary>
    public class FFmpegVideoTransmissionService : IVideoTransmissionService
    {
        const int BITRATE = 1600000;

        IOverkillConfiguration config;

        public FFmpegVideoTransmissionService(IOverkillConfiguration _configuration)
        {
            config = _configuration;

            FFmpeg.SetExecutablesPath(config.Streaming.FFmpegExecutablePath);
        }

        /// <summary>
        /// Compiles a set of FFmpeg arguments and starts a process
        /// </summary>
        public async Task Start()
        {
            try
            {
                var ffmpeg = FFmpeg.Conversions.New();
                var arguments = ffmpeg
                    .AddParameter($"-i {config.Streaming.Devices[0]}", ParameterPosition.PreInput)
                    .AddParameter("-f alsa", ParameterPosition.PreInput)
                    .AddParameter("-i hw:1,0", ParameterPosition.PreInput)
                    .AddParameter("-f mpegts", ParameterPosition.PostInput)
                    .AddParameter("-vcodec mpeg1video", ParameterPosition.PostInput)
                    .AddParameter("-pix_fmt yuv420p", ParameterPosition.PostInput)
                    .AddParameter("-acodec mp2", ParameterPosition.PostInput)
                    .AddParameter("-ar 48000", ParameterPosition.PostInput)
                    .AddParameter("-ac 2", ParameterPosition.PostInput)
                    .AddParameter("-b:a 128k", ParameterPosition.PostInput)
                    .AddParameter("-preset superfast", ParameterPosition.PostInput)
                    .AddParameter("-tune zerolatency", ParameterPosition.PostInput)
                    .AddParameter("-fflags nobuffer", ParameterPosition.PostInput)
                    .AddParameter("-s 640x480", ParameterPosition.PostInput)
                    .AddParameter("-analyzeduration 1", ParameterPosition.PostInput)
                    .AddParameter("-probesize 32", ParameterPosition.PostInput)
                    .AddParameter($"-user-agent \"{config.System.AuthorizationToken}\"")
                    .SetVideoBitrate(BITRATE)
                    .SetOutput(config.Streaming.Endpoint)
                    .Build();

                Console.WriteLine(arguments);

                await ffmpeg.Start(arguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
