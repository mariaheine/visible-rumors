using System;
using System.Linq;
using System.Text;
using UnityEngine;
using static VisibleRumors.Extensions;

namespace VisibleRumors
{
    public class AudioPeer : MonoBehaviour
    {
        [SerializeField] public TrackProfile trackProfile;

        [SerializeField] float _bandBoost = 10f;
        [SerializeField, Range(1, 100)] float _bufferDecreaseStartingSpeed = 5f;
        [SerializeField] GameObject _spectrumProviderObject;

        public float[] audioRanged = new float[8];
        public float[] audioRangedBuffer = new float[8];
        public float[] freqBandHighest; // rather readonly to compare against audio profile
        public float audioProfile;
        public float amplitudeRanged;
        public float amplitudeBufferRanged;

        ISprectrumProvideable _spectrumProvider;

        int[] _sampleCountPerBand;
        public float[] _freqBands = new float[8];
        float[] _bandBuffers = new float[8];
        float[] _bufferDecreaseSpeed = new float[8];
        float _maxAmplitude;

        public float[] SpectrumData
        {
            get
            {
                if (_spectrumProvider is null)
                {
                    _spectrumProvider = GetSpectumProvider();
                }

                return _spectrumProvider.GetSpectrumData();
            }
        }

        //todo: split entire class into track profile recorder and player
        //? or implement strategy if you would still like to be able to do live stuff
        //* for live it would be nice to have some commands; like issue printable words with 
        //* animation


        void Awake()
        {
            if (_spectrumProvider is null)
            {
                _spectrumProvider = GetSpectumProvider();
            }

            freqBandHighest = new float[_freqBands.Length];

            InitAudioProfile();
            CalculateSampleCountPerBand(ref _sampleCountPerBand, _spectrumProvider);
        }

        ISprectrumProvideable GetSpectumProvider()
        {
            ISprectrumProvideable spectrumProvider;

            if (!_spectrumProviderObject.TryExtractInterface<ISprectrumProvideable>(out spectrumProvider))
            {
                Debug.LogError("AudioPeer requires a reference to a spectrum provider object", transform);
                return null;
            }
            else
            {
                return spectrumProvider;
            }
        }

        void InitAudioProfile()
        {
            // it only solves the problem of init values for bands
            // amplitudes could use a similar solution
            //? but how would you do it live
            for (int i = 0; i < freqBandHighest.Length; i++)
            {
                freqBandHighest[i] = audioProfile;
            }
        }

        void Update()
        {
            MakeFrequencyBands(SpectrumData, _freqBands, _bandBoost, _sampleCountPerBand);
            BandBuffer();
            CreateRangedBuffers();
            GetAmplitude();
        }


        static void CalculateSampleCountPerBand(ref int[] sampleCountPerBand, ISprectrumProvideable spectrumProvider)
        {
            // -------------

            /*
                The generally established audio frequency range is 20 Hz to 20,000 Hz

                Sub-bass 	        16 to 60 Hz         - an upright bass, tuba, bass guitar
                Bass 	            60 to 250 Hz        - normal speaking vocal range
                Lower Midrange 	    250 to 500 Hz       - brass instruments, and mid woodwinds, like alto saxophone and the middle range of a clarinet
                Midrange 	        500 Hz to 2 kHz     - higher end of the fundamental frequencies created by most musical instruments
                Higher Midrange 	2 to 4 kHz
                Presence 	        4 to 6 kHz
                Brilliance 	        6 to 20 kHz

                taken from : https://www.cuidevices.com/blog/understanding-audio-frequency-range-in-audio-design

                Thus we divide 20,000/x where x is the number of samples
            */

            sampleCountPerBand = new int[7];
            int[] bandRanges = new int[] { 60, 180, 250, 1500, 2000, 2000, 14000 };
            int spectrumSize = spectrumProvider.GetSpectrumSize();
            int sampleWidth = 20000 / spectrumSize;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 7; i++)
            {
                int count = bandRanges[i] / sampleWidth;

                sampleCountPerBand[i] = count;

                sb.AppendLine($"band {i}: {count}");
            }

            int sampleCountSum = sampleCountPerBand.Sum();
            if (sampleCountSum != spectrumSize)
            {
                sampleCountPerBand[6] -= -(spectrumSize - sampleCountSum);
            }

            Debug.Log($"{sb.ToString()}");
            // Debug.Log(sampleCountPerBand.Aggregate((temp, x) => temp + x));


            // -------------
        }

        static void MakeFrequencyBands(float[] spectrumData, float[] freqBands, float bandBoost, int[] sampleCountPerBand)
        {
            // -------------

            int spectrumIndex = 0;

            // for (int i = 0; i < 7; i++)
            for (int i = 0; i < 7; i++)
            {
                float average = 0;

                for (int j = 0; j < sampleCountPerBand[i]; j++)
                // for (int j = 0; j < 512 / 8; j++)
                {
                    average += spectrumData[spectrumIndex];// * (spectrumIndex + 1); // why? probably to boost highs visibility
                    spectrumIndex++;
                }

                // average /= sampleCountPerBand[i];
                average /= (spectrumData.Length / 8);
                freqBands[i] = average * bandBoost;
            }

            // -------------
        }

        private void GetAmplitude()
        {
            float currentAmplitude = 0;
            float currentAmplitudeBuffer = 0;

            for (int i = 0; i < _freqBands.Length; i++)
            {
                currentAmplitude += _freqBands[i];
                currentAmplitudeBuffer += _bandBuffers[i];
            }

            if (currentAmplitudeBuffer > _maxAmplitude)
            {
                _maxAmplitude = currentAmplitudeBuffer;
            }

            amplitudeRanged = currentAmplitude / _maxAmplitude;
            amplitudeBufferRanged = currentAmplitudeBuffer / _maxAmplitude;
        }

        private void CreateRangedBuffers()
        {
            for (int i = 0; i < _freqBands.Length; i++)
            {
                if (_freqBands[i] > freqBandHighest[i])
                {
                    freqBandHighest[i] = _freqBands[i];
                }

                audioRanged[i] = _freqBands[i] / freqBandHighest[i];
                audioRangedBuffer[i] = _bandBuffers[i] / freqBandHighest[i];
            }
        }

        private void BandBuffer()
        {
            for (int i = 0; i < _bandBuffers.Length; i++)
            {
                if (_bandBuffers[i] < _freqBands[i])
                {
                    _bandBuffers[i] = _freqBands[i];
                    _bufferDecreaseSpeed[i] = _bufferDecreaseStartingSpeed / 10000f;
                }
                else
                {
                    _bandBuffers[i] -= _bufferDecreaseSpeed[i];
                    _bufferDecreaseSpeed[i] *= 1.2f;
                }
            }
        }
    }

}
