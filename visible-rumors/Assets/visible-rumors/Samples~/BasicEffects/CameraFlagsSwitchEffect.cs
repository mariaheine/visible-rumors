using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class CameraFlagsSwitchEffect : IAudioEffect
    {
        [Space, Header("Camera Flag Switch")]
        [SerializeField] Camera targetCamera;
        [SerializeField] CameraClearFlags activeFlags;
        [SerializeField] CameraClearFlags disabledFlags;

        public override void OnEffectActivate()
        {
            targetCamera.clearFlags = activeFlags;
        }

        public override void OnEffectDeactivate()
        {
            targetCamera.clearFlags = disabledFlags;
        }
    }
}