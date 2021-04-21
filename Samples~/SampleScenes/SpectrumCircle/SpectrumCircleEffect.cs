using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class SpectrumCircleEffect : BaseAudioEffect
    {
        [SerializeField] GameObject _cubePrefab;
        [SerializeField] float _maxScale;

        GameObject[] _cubes;
        int _spectrumLength;

        protected override void OnEffectInitialize()
        {
            _spectrumLength = SpectrumData.Length;
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

        protected override void OnEffectUpkeepTick()
        {
            for(int i = 0; i < 512; i++)
            {
                _cubes[i].transform.localScale = new Vector3(1, SpectrumData[i] * _maxScale, 1);
            }
        }
    }
}
