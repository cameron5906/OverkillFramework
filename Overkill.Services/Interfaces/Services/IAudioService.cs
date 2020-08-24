namespace Overkill.Services.Interfaces.Services
{
    public interface IAudioService
    {
        void PlayAudioFromURL(string url);
        void PlayFromLocalFile(string localFile);
    }
}
