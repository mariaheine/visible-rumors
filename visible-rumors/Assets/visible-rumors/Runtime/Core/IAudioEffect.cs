using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    [RequireComponent(typeof(AudioEffectBase))]
    public abstract class IAudioEffect : MonoBehaviour
    {
        protected IAudioEffectDataProvider _audioData;

        public virtual void OnEffectInitialize() { }
        public virtual void OnEffectActivate() { }
        public virtual void OnEffectDeactivate() { }
        public virtual void OnEffectUpkeepTick() { }

        public void Initialize(IAudioEffectDataProvider audioDataProvider)
        {
            _audioData = audioDataProvider;
        }
        
        protected void Awake() { } // Use OnEffectInitialize instead
        protected void Start() { } // Use OnEffectInitialize instead
        protected void OnDisable() { } // Use OnEffectDeactivate instead
        protected void Update() { } // Use OnEffectUpkeepTick instead
    }
}
