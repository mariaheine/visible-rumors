using Assets.WasapiAudio.Scripts.Unity;

namespace VisibleRumors
{
    public class WasapiAudioSourceAdapter : WasapiAudioSource, ISprectrumProvideable
    {
        public int GetSpectrumSize()
        {
            return SpectrumSize;
        }
    }
}
