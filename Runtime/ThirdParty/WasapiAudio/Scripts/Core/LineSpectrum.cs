using System.Collections.Generic;
using System.Linq;
using CSCore.DSP;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Core
{
    internal class LineSpectrum : SpectrumBase
    {
        public int BarCount
        {
            get => SpectrumResolution;
            set => SpectrumResolution = value;
        }

        public LineSpectrum(FftSize fftSize, int minFrequency, int maxFrequency)
        : base(minFrequency, maxFrequency)
        {
            FftSize = fftSize;
        }

        public float[] GetSpectrumData(double maxValue)
        {
            // Get spectrum data internal
            var fftBuffer = new float[(int)FftSize];

            UpdateFrequencyMapping();

            if (SpectrumProvider.GetFftData(fftBuffer, this))
            {
                SpectrumPointData[] spectrumPoints = CalculateSpectrumPoints(maxValue, fftBuffer);
                float[] spectrumData = new float[spectrumPoints.Length];

                for(int i = 0; i < spectrumPoints.Length; i++)
                {
                    spectrumData[i] = (float)spectrumPoints[i].Value;
                }

                return spectrumData.ToArray();
            }

            // if (SpectrumProvider.GetFftData(fftBuffer, this))
            // {
            //     return fftBuffer;
            // }
            return null;
        }
    }
}