using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors
{
    [RequireComponent(typeof(AudioSource))]
    public class TrackPlayer : MonoBehaviour
    {
        public TrackProfile trackProfile;

        AudioSource _audioSource;
        AudioPeer _audioPeer;

        void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioPeer = GetComponent<AudioPeer>();
            _audioSource.clip = trackProfile.audioClip;
            _audioSource.time = 0f;

            StartCoroutine(PlayDelayed());
        }

        IEnumerator PlayDelayed()
        {
            yield return new WaitForSeconds(3);

            _audioSource.Play();
        }
    }
}
