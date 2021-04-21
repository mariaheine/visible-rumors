using UnityEngine;
using System;
using System.Collections;

namespace VisibleRumors
{
    public abstract class BaseAudioEffect : MonoBehaviour
    {
        [System.Serializable]
        public class AmplitudeTreshold
        {
            public bool isEnabed;
            public bool useBuffer;
            [Range(0, 1)] public float treshold;
        }

        [System.Serializable]
        public class BandTreshold
        {
            public bool isEnabed;
            public bool useBuffer;
            public int band;
            [Range(0, 1)] public float treshold;
        }

        // separate trigger interface from activation interface

        // also implement an interface for timed manipulation of value by enumerated easing function

        [Space, Header("Base Audio Effect Settings")]
        public KeyCode keyCode = KeyCode.None;
        public bool isToggle = true;
        public bool startsTriggered = true;
        public AmplitudeTreshold amplitudeTreshold;
        public BandTreshold bandTreshold;
        public BaseAudioEffect parentEffect;

        [Space, Header("Readonly please")]
        public bool isEnabled = false;
        public bool isActive = false;


        private Action stateFlipped;

        protected virtual void OnEffectInitialize() { }
        protected virtual void OnEffectActivate() { }
        protected virtual void OnEffectDeactivate() { }
        protected virtual void OnEffectUpkeepTick() { }

        protected float[] SpectrumData => audioPeer.SpectrumData;

        // That probably shouldnt be here
        protected float RangedAmplitude { get; private set; }
        protected float RangedBuffer { get; private set; }

        AudioPeer audioPeer;
        bool isInitialized = false;

        void Awake()
        {
            audioPeer = GameObject.FindObjectOfType(typeof(AudioPeer)) as AudioPeer;

            if (audioPeer == null)
            {
                Debug.LogError("audium peerum null");
            }
            else
            {
                InitializeEffect();

                // this is problematic
                stateFlipped = () =>
                {
                    if (isActive == true)
                    {
                        OnEffectActivate();
                    }
                    else
                    {
                        OnEffectDeactivate();
                    }
                };

                if (startsTriggered)
                {
                    isEnabled = true;
                    isActive = true;
                }
            }
        }

        void OnDisable()
        {
            if (isActive)
            {
                isActive = false;
                OnEffectDeactivate();
            }
        }

        void Update()
        {
            if (!isInitialized) return;

            // Move it :d
            // Currently I do a lot of spaghetti, after this project maybe I will know better
            // how to plan the structure, interface and features.
            RangedAmplitude = audioPeer.amplitudeRanged;
            RangedBuffer = audioPeer.amplitudeBufferRanged;

            if (Input.GetKeyDown(keyCode))
            {
                Debug.Log(keyCode, transform);
                if (!isEnabled)
                {
                    isEnabled = true;
                }
                else
                {
                    isEnabled = false;
                    isActive = false; // stateFlipped needs rework
                    OnEffectDeactivate();
                }
            }

            if (Input.GetKeyUp(keyCode))
            {
                if (!isToggle && isEnabled)
                {
                    isEnabled = false;
                    if (isActive)
                    {
                        isActive = false;
                        OnEffectDeactivate();
                    }
                }
            }

            if (isEnabled)
            {
                if (amplitudeTreshold.isEnabed || bandTreshold.isEnabed)
                {
                    CheckTresholds();
                }

                if (isActive)
                {
                    OnEffectUpkeepTick();
                }
            }
        }

        void InitializeEffect()
        {
            if (SpectrumData == null)
            {
                StartCoroutine(WaitForSpectrumData());
            }
            else
            {
                OnEffectInitialize();
                isInitialized = true;
            }
        }

        IEnumerator WaitForSpectrumData()
        {
            yield return new WaitForSeconds(0.1f);
            InitializeEffect();
        }

        private void CheckTresholds()
        {
            bool amplitudeThrough = false;
            bool bandThrough = false;

            if (parentEffect == null || parentEffect.isActive)
            {
                // Amplitude
                if (amplitudeTreshold.isEnabed)
                {
                    if (amplitudeTreshold.useBuffer)
                    {
                        if (audioPeer.amplitudeBufferRanged > amplitudeTreshold.treshold)
                        {
                            amplitudeThrough = true;
                        }
                    }
                    else
                    {
                        if (audioPeer.amplitudeRanged > amplitudeTreshold.treshold)
                        {
                            amplitudeThrough = true;
                        }
                    }
                }

                // Band
                if (bandTreshold.isEnabed)
                {
                    if (bandTreshold.useBuffer)
                    {
                        if (audioPeer.audioRangedBuffer[bandTreshold.band] > bandTreshold.treshold)
                        {
                            bandThrough = true;
                        }
                    }
                    else
                    {
                        if (audioPeer.audioRanged[bandTreshold.band] > amplitudeTreshold.treshold)
                        {
                            bandThrough = true;
                        }
                    }
                }
            }

            bool isThrough = false;

            if (amplitudeTreshold.isEnabed && bandTreshold.isEnabed)
            {
                isThrough = amplitudeThrough && bandThrough;
            }
            else
            {
                isThrough = amplitudeThrough || bandThrough;
            }

            if (isThrough != isActive)
            {
                isActive = isThrough;
                stateFlipped.Invoke();
            }
        }
    }
}