using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using static VisibleRumors.Extensions;

namespace VisibleRumors
{
    public class AudioPeer : MonoBehaviour
    {
        [SerializeField] IAudioEffectDataProvider _defaultAudioEffectDataProvider;
        [SerializeField] GameObject _spectrumProviderObject;

        ISprectrumProvideable _spectrumProvider;
        bool _isAudioPeerReady = false;

        public delegate void onAudioPeerReady();
        onAudioPeerReady onReadyDelegates;

        public IAudioEffectDataProvider DefaultAudioEffectDataProvider => _defaultAudioEffectDataProvider;
        public float[] SpectrumData => _spectrumProvider.GetSpectrumData();
        public int SpectrumSize => _spectrumProvider.GetSpectrumSize();

        public bool IsAudioPeerReady
        {
            get => _isAudioPeerReady;
            private set
            {
                if (_isAudioPeerReady == false && value == true)
                {
                    onReadyDelegates?.Invoke();
                }

                _isAudioPeerReady = value;
            }
        }

        public void DoOnAudioPeerReady(onAudioPeerReady onReady)
        {
            // -------------
            
            if (!IsAudioPeerReady)
            {
                onReadyDelegates += onReady;
            }
            else
            {
                onReady.Invoke();
            }
            
            // -------------
        }

        void Start()
        {
            // -------------

            if (_defaultAudioEffectDataProvider == null)
            {
                Debug.LogError("Audio Peer is missing a Default Adio Effect Data Provider, thats not good.");
            }
            else
            {
                _defaultAudioEffectDataProvider.Initialize(this);
            }

            _spectrumProvider = GetSpectumProvider();
            CheckSpectrumData(0f);

            // -------------
        }

        ISprectrumProvideable GetSpectumProvider()
        {
            // -------------

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

            // -------------
        }

        void CheckSpectrumData(float waitTime)
        {
            // -------------
            
            if (waitTime > 5f)
            {
                Debug.Log("Already waiting 5 seconds for the spectrum provider to give correct spectrum data.");
            }
            else if (waitTime > 15f)
            {
                Debug.LogError("Already waited 15 seconds for the spectrum provider to give correct spectrum data, red alert, something is dead, wait why is it so silent, hey where is everyone, omg someone pls help, aurelia to enterprise, do you receive me, oh worf omg at least you are here, where were you, yes my phaser is working, set to kill, what are these things, yes i will follow you, so good you are here, when will they beam us up, when did the signal die, wait did you hear that, oh that hurts, happy to die in your arms worf, no i'm joking i won't fall asleep");
                return;
            }
            
            if (SpectrumData == null)
            {
                StartCoroutine(WaitForSpectrumData(waitTime));
            }
            else
            {
                IsAudioPeerReady = true;
            }
            
            // -------------
        }

        IEnumerator WaitForSpectrumData(float waitTime)
        {
            // -------------

            float delay = 0.1f;
            yield return new WaitForSeconds(delay);
            CheckSpectrumData(waitTime + delay);

            // -------------
        }
    }
}
