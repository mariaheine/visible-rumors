using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class SpectrumCircleEffect : IAudioEffect
    {
        [SerializeField] GameObject _cubePrefab;
        [SerializeField] float _maxScale;

        GameObject[] _cubes;
        int _spectrumLength;

        public override void OnEffectInitialize()
        {
            _spectrumLength = _audioData.SpectrumData.Length;
            _cubes = new GameObject[_spectrumLength];
            float rot = 360f / _spectrumLength;

            for(int i = 0; i < _spectrumLength; i++)
            {
                GameObject cube = (GameObject)Instantiate(_cubePrefab);
                cube.transform.parent = transform;
                cube.name = $"Cube_{i}";
                cube.transform.localPosition = Quaternion.Euler(0,rot*i,0) * transform.forward * 100;
                _cubes[i] = cube;
            }
        }

        public override void OnEffectUpkeepTick()
        {
            for(int i = 0; i < 512; i++)
            {
                _cubes[i].transform.localScale = new Vector3(1, _audioData.SpectrumData[i] * _maxScale, 1);
            }
        }
    }
}
