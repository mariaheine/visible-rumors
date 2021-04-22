using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class EnableObjectEffect : IAudioEffect
    {
        [Space, Header("Enable Object Effect")]
        [SerializeField] GameObject rootObject;

        [Header("References")]
        [SerializeField] bool startsEnabled;

        public override void OnEffectInitialize()
        {
            rootObject.SetActive(startsEnabled);
        }

        public override void OnEffectActivate()
        {
            rootObject.SetActive(true);
        }

        public override void OnEffectDeactivate()
        {
            rootObject.SetActive(false);
        }
    }

}