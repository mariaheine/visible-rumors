using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    [RequireComponent(typeof(AudioSource))]
    public class UnityAudioSource : MonoBehaviour, ISprectrumProvideable
    {
        [SerializeField, Tooltip("This has to be divisible by 2")] 
        int _spectrumSize = 64;

        [SerializeField]
        FFTWindow _fftWindow = FFTWindow.Blackman;

        AudioSource _audioSource;
        float[] _spectrumData;

        void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            if (_spectrumSize % 2 != 0)
            {
                Debug.LogError("Spectrum Size should be a number divisible by two", transform);
            }

            _spectrumData = new float[_spectrumSize];
        }

        void Update()
        {
            _audioSource.GetSpectrumData(_spectrumData, 0, _fftWindow);
        }
        
        public float[] GetSpectrumData()
        {
            return _spectrumData;
        }

        public int GetSpectrumSize()
        {
            return _spectrumSize;
        }
    }
}