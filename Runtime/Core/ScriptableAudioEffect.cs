using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    public abstract class ScriptableAudioEffect : ScriptableObject
    {
        public virtual void OnEffectInstantiate() { }
        public virtual void OnEffectActivate() { }
        public virtual void OnEffectDeactivate() { }
        public virtual void OnEffectUpkeepTick() { }
    }
}
