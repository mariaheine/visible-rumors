using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class CameraFlagsSwitchEffect : BaseAudioEffect
    {
        [Space, Header("Camera Flag Switch")]
        [SerializeField] Camera targetCamera;
        [SerializeField] CameraClearFlags activeFlags;
        [SerializeField] CameraClearFlags disabledFlags;

        protected override void OnEffectActivate()
        {
            targetCamera.clearFlags = activeFlags;
        }

        protected override void OnEffectDeactivate()
        {
            targetCamera.clearFlags = disabledFlags;
        }
    }
}