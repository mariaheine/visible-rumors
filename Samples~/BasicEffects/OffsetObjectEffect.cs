using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class OffsetObjectEffect : IAudioEffect
    {
        // TODO
        // Could add chainable randomize offset value effect inbetween

        [Space, Header("Offset Object")]
        [SerializeField] List<Transform> targets;
        [SerializeField] Vector3 offset;

        public override void OnEffectActivate()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].position += offset;
            }
        }

        public override void OnEffectDeactivate()
        {
            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].position -= offset;
            }
        }
    }

}