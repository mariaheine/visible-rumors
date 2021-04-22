using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VisibleRumors
{
    public class DefaultAudioDataProvider : IAudioEffectDataProvider
    {
        [SerializeField] float _bandBoost = 10f;
        [SerializeField] bool _useBandBuffer = true;
        [SerializeField, Range(1, 100)] float _bufferDecreaseStartingSpeed = 5f;
        [SerializeField] float _audioProfile = 5f; // TODO this is kindof random, just adding that as starting highest band value

        int[] _sampleCountPerBand;
        float[] _band = new float[7];
        float[] _bandRanged = new float[7];
        float[] _bandBuffer = new float[7];
        float[] _bandBufferRanged = new float[7];
        float[] _bufferDecreaseSpeed = new float[7];
        float[] _freqBandHighest; // rather readonly to compare against audio profile

        float _amplitudeRanged;
        float _amplitudeBufferRanged;
        float _maxAmplitude;

        public override float[] SpectrumData
        {
            get 
            {
                if (AudioPeer == null) return null;
                else return AudioPeer.SpectrumData;
            }
        }

        public override int? SpectrumSize
        {
            get 
            {
                if (AudioPeer == null) return null;
                else return AudioPeer.SpectrumSize;
            }
        }

        public override float[] BandData => _band;
        public override float[] BandDataRanged => _bandRanged;
        public override float[] BandDataBuffered => _bandBuffer;
        public override float AmplitudeRanged => _amplitudeRanged;
        public override float AmplitudeBufferRanged => _amplitudeBufferRanged;

        protected override void InitializeAudioData()
        {
            // -------------

            InitAudioProfile();
            CalculateSampleCountPerBand(ref _sampleCountPerBand, SpectrumSize.Value);

            // -------------
        }

        void InitAudioProfile()
        {
            // -------------
            
            _freqBandHighest = new float[_band.Length];

            for (int i = 0; i < _freqBandHighest.Length; i++)
            {
                _freqBandHighest[i] = _audioProfile;
            }
            
            // -------------
        }

        static void CalculateSampleCountPerBand(ref int[] sampleCountPerBand, int spectrumSize)
        {
            // -------------

            // TODO Unfortunately this looks nice on paper but does not work at all with the WASAPI loopbac specturm data
            // Does it even work at all
            // Well, there are 7 arbitrary bands

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
            int[] bandRanges = new int[] { 60, 190, 250, 1500, 2000, 2000, 14000 };
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

            // -------------
        }

        protected override void UpdateAudioData()
        {
            // -------------

            MakeFrequencyBands(SpectrumData, _band, _bandBoost, _sampleCountPerBand);
            BandBuffer();
            CreateRangedBuffers();
            GetAmplitude();
            
            // -------------
        }

        static void MakeFrequencyBands(float[] spectrumData, float[] freqBands, float bandBoost, int[] sampleCountPerBand)
        {
            // -------------

            int spectrumIndex = 0;

            for (int i = 0; i < 7; i++)
            {
                float average = 0;

                for (int j = 0; j < sampleCountPerBand[i]; j++)
                {
                    average += spectrumData[spectrumIndex];// * (spectrumIndex + 1); // why? probably to boost highs visibility
                    spectrumIndex++;
                }

                average /= (spectrumData.Length / 7);
                freqBands[i] = average * bandBoost;
            }

            // -------------
        }

        void BandBuffer()
        {
            // -------------

            for (int i = 0; i < _bandBuffer.Length; i++)
            {
                if (_bandBuffer[i] < _band[i])
                {
                    _bandBuffer[i] = _band[i];
                    _bufferDecreaseSpeed[i] = _bufferDecreaseStartingSpeed / 10000f;
                }
                else
                {
                    _bandBuffer[i] -= _bufferDecreaseSpeed[i];
                    _bufferDecreaseSpeed[i] *= 1.2f;
                }
            }

            // -------------
        }

        void CreateRangedBuffers()
        {
            // -------------

            for (int i = 0; i < _band.Length; i++)
            {
                if (_band[i] > _freqBandHighest[i])
                {
                    _freqBandHighest[i] = _band[i];
                }

                _bandRanged[i] = _band[i] / _freqBandHighest[i];
                _bandBufferRanged[i] = _bandBuffer[i] / _freqBandHighest[i];
            }

            // -------------
        }

        void GetAmplitude()
        {
            // -------------

            float currentAmplitude = 0;
            float currentAmplitudeBuffer = 0;

            for (int i = 0; i < _band.Length; i++)
            {
                currentAmplitude += _band[i];
                currentAmplitudeBuffer += _bandBuffer[i];
            }

            if (currentAmplitudeBuffer > _maxAmplitude)
            {
                _maxAmplitude = currentAmplitudeBuffer;
            }

            _amplitudeRanged = currentAmplitude / _maxAmplitude;
            _amplitudeBufferRanged = currentAmplitudeBuffer / _maxAmplitude;

            // -------------
        }
    }
}
