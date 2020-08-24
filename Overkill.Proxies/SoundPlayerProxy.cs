using CSCore;
using CSCore.Codecs;
using CSCore.SoundOut;
using Overkill.Proxies.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Overkill.Proxies
{
    /// <summary>
    /// Proxy class to assist in unit testing sound playback functionality
    /// </summary>
    public class SoundPlayerProxy : ISoundPlayerProxy
    {
        ISoundOut soundOut;

        public SoundPlayerProxy(ISoundOut _soundOut)
        {
            soundOut = _soundOut;
        }

        public void Play(string fileName)
        {
            var waveSource = CodecFactory.Instance
                .GetCodec(fileName)
                .ToSampleSource()
                .ToMono()
                .ToWaveSource();

            soundOut.Initialize(waveSource);
            soundOut.Play();
        }
    }
}
