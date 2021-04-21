using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    /* Created at 22 January 2020 by mria 🌊🐱 */
    [CreateAssetMenu(fileName = "TrackProfile.asset", menuName = "Visualisation/TrackProfile")]
    public class TrackProfile : ScriptableObject
    {
        public AudioClip audioClip;
        public float maxAmplitude;
        public float[] freqBandHighest;

        public bool recordData;
    }
}
