using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class BandCubesEffect : IAudioEffect
    {
        [SerializeField] GameObject[] _bandCubes = new GameObject[7];
        [SerializeField] float _scaleMultiplier = 1f;
        
        public override void OnEffectUpkeepTick()
        {
            for(int i = 0; i < 7; i++)
            {
                Vector3 localScale = _bandCubes[i].transform.localScale;
                _bandCubes[i].transform.localScale = new Vector3(localScale.x, _audioData.BandData[i] * _scaleMultiplier, localScale.z);
            }
        }
    }
}
