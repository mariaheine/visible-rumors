using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    public class BandCube : MonoBehaviour
    {
        [SerializeField] int _band;
        [SerializeField] float _scaleMultiplier;
        [SerializeField] AudioPeer _audioPeer;

        void Update()
        {
            Vector3 localScale = transform.localScale;
            transform.localScale = new Vector3(localScale.x, _audioPeer._freqBands[_band] * _scaleMultiplier, localScale.z);
        }
    }
}
