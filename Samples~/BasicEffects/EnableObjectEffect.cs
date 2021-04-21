using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class EnableObjectEffect : BaseAudioEffect
    {
        [Space, Header("Enable Object Effect")]
        [SerializeField] GameObject rootObject;
        [SerializeField] GameObject followedSpirit;

        [Header("References")]
        [SerializeField] bool startsEnabled;

        // TODO
        // write a setup to make those chainable separate effects
        // separate audio triggerer from the effect itself
        // making effects depend on one another
        // CHAINS

        void Awake()
        {
            rootObject.SetActive(startsEnabled);
        }

        protected override void OnEffectActivate()
        {
            rootObject.SetActive(true);
        }

        protected override void OnEffectDeactivate()
        {
            rootObject.SetActive(false);
        }
    }

}