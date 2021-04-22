using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    public class IAudioEffectDataProvider : MonoBehaviour
    {
        AudioPeer _audioPeer;
        bool _isInitialized;

        public virtual float[] SpectrumData { get; }
        public virtual int? SpectrumSize { get; }
        public virtual float[] BandData  { get; }
        public virtual float[] BandDataRanged  { get; }
        public virtual float[] BandDataBuffered  { get; }
        public virtual float AmplitudeRanged  { get; }
        public virtual float AmplitudeBufferRanged  { get; }
        
        protected AudioPeer AudioPeer => _audioPeer;
        protected virtual void InitializeAudioData() { }
        protected virtual void UpdateAudioData() { }

        public void Initialize(AudioPeer audioPeer)
        {
            // -------------
            
            _audioPeer = audioPeer;

            _audioPeer.DoOnAudioPeerReady(() => {
                InitializeAudioData();
                _isInitialized = true;
            });
            
            // -------------
        }

        protected void Update()
        {
            // -------------
            
            if (_isInitialized)
            {
                UpdateAudioData();
            }
            
            // -------------
        }
    }
}
