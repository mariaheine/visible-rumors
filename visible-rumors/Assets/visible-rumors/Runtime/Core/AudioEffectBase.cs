using UnityEngine;
using System;
using System.Collections;

namespace VisibleRumors
{
    public class AudioEffectBase : MonoBehaviour
    {
        [Serializable]
        class AmplitudeTreshold
        {
            public bool isEnabed;
            public bool useBuffer;
            [Range(0, 1)] public float treshold;
        }

        [Serializable]
        class BandTreshold
        {
            public bool isEnabed;
            public bool useBuffer;
            public int band;
            [Range(0, 1)] public float treshold;
        }

        [Space, Header("Base Audio Effect Settings")]
        [SerializeField] KeyCode _keyCode = KeyCode.None;
        [SerializeField] bool _isToggle = true;
        [SerializeField] bool _startsEnabled = true;
        [SerializeField] AmplitudeTreshold _amplitudeTreshold;
        [SerializeField] BandTreshold _bandTreshold;
        [SerializeField] AudioEffectBase _parentEffect;
        [SerializeField] IAudioEffectDataProvider _dataProviderOverride; // TODO implement

        // TODO Add a Readonly Attribute
        [Space, Header("Readonly please, don't change")]
        [SerializeField] bool isEnabled = false;
        [SerializeField] bool isActive = false;

        AudioPeer _audioPeer;
        IAudioEffect _audioEffect;
        IAudioEffectDataProvider _audioEffectDataProvider;
        bool isInitialized = false;

        protected bool IsActive
        {
            get
            {
                return isActive;
            }
            private set
            {
                if (value == isActive) return;

                if (value)
                {
                    isActive = true;
                    _audioEffect.OnEffectActivate();
                }
                else
                {
                    isActive = false;
                    _audioEffect.OnEffectDeactivate();
                }
            }
        }

        void Awake()
        {
            // -------------
            
            // TODO Rework how AudioPeer is obtained
            _audioPeer = GameObject.FindObjectOfType(typeof(AudioPeer)) as AudioPeer;

            if (_audioPeer == null)
            {
                Debug.LogError("Couldn't find an Audio Peer");
            }
            else
            {
                InitializeEffectDataProvider();
                TryInitializeEffect();
            }

            // -------------
        }

        void InitializeEffectDataProvider()
        {
            // -------------
            
            if (_dataProviderOverride != null)
            {
                _dataProviderOverride.Initialize(_audioPeer);
                _audioEffectDataProvider = _dataProviderOverride;
            }
            else
            {
                _audioEffectDataProvider = _audioPeer.DefaultAudioEffectDataProvider;
            }
            
            // -------------
        }

        void TryInitializeEffect()
        {
            // -------------

            _audioPeer.DoOnAudioPeerReady(InitializeEffect);
            
            // -------------
        }

        void InitializeEffect()
        {
            // -------------
            
            if (_audioPeer.SpectrumData == null)
            {
                StartCoroutine(WaitForSpectrumData());
            }
            else
            {
                _audioEffect = GetComponent<IAudioEffect>();
                _audioEffect.Initialize(_audioEffectDataProvider);
                _audioEffect.OnEffectInitialize();

                isInitialized = true;

                if (_startsEnabled)
                {
                    isEnabled = true;
                }
            }
            
            // -------------
        }

        IEnumerator WaitForSpectrumData()
        {
            // -------------
            
            yield return new WaitForSeconds(0.1f);
            InitializeEffect();
            
            // -------------
        }

        void Update()
        {
            // -------------
            
            if (!isInitialized) return;

            if (Input.GetKeyDown(_keyCode))
            {
                isEnabled = !isEnabled;
            }

            //* Re-disable on key up if it is not a toggle
            if (Input.GetKeyUp(_keyCode))
            {
                if (!_isToggle)
                {
                    isEnabled = !isEnabled;
                }
            }

            if (isEnabled && (_parentEffect == null || _parentEffect.IsActive))
            {
                if (_amplitudeTreshold.isEnabed || _bandTreshold.isEnabed)
                {
                    CheckTresholds();
                }
                else
                {
                    //* If both thresholds are disabled then we assume the effect to be always active
                    IsActive = true;
                }

                if (IsActive)
                {
                    _audioEffect.OnEffectUpkeepTick();
                }
            }
            else
            {
                IsActive = false;
            }
            
            // -------------
        }

        void CheckTresholds()
        {
            // -------------
            
            bool isThrough = false;

            bool amplitudeThrough = CheckAmplitudeTreshold();
            bool bandThrough = CheckBandTreshold();

            if (_amplitudeTreshold.isEnabed && _bandTreshold.isEnabed)
            {
                isThrough = amplitudeThrough && bandThrough;
            }
            else
            {
                isThrough = amplitudeThrough || bandThrough;
            }

            if (isThrough != IsActive)
            {
                IsActive = isThrough;
            }
            
            // -------------
        }

        bool CheckAmplitudeTreshold()
        {
            // -------------

            bool amplitudeThrough = false;
            
            if (_amplitudeTreshold.isEnabed)
            {
                if (_amplitudeTreshold.useBuffer)
                {
                    if (_audioEffectDataProvider.AmplitudeBufferRanged > _amplitudeTreshold.treshold)
                    {
                        amplitudeThrough = true;
                    }
                }
                else
                {
                    if (_audioEffectDataProvider.AmplitudeRanged > _amplitudeTreshold.treshold)
                    {
                        amplitudeThrough = true;
                    }
                }
            }

            return amplitudeThrough;
            
            // -------------
        }

        bool CheckBandTreshold()
        {
            // -------------

            bool bandThrough = false;

            if (_bandTreshold.isEnabed)
            {
                if (_bandTreshold.useBuffer)
                {
                    if (_audioEffectDataProvider.BandDataBuffered[_bandTreshold.band] > _bandTreshold.treshold)
                    {
                        bandThrough = true;
                    }
                }
                else
                {
                    if (_audioEffectDataProvider.BandData[_bandTreshold.band] > _amplitudeTreshold.treshold)
                    {
                        bandThrough = true;
                    }
                }
            }

            return bandThrough;
            
            // -------------
        }
    }
}