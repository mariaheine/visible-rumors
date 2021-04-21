using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class OffsetObjectEffect : BaseAudioEffect
    {
        // TODO
        // Could add chainable randomize offset value effect inbetween

        [Space, Header("Offset Object")]
        [SerializeField] List<Transform> targets;
        [SerializeField] Vector3 offset;

        protected override void OnEffectActivate()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].position += offset;
            }
        }

        protected override void OnEffectDeactivate()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].position -= offset;
            }
        }
    }

}