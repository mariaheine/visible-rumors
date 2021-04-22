using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VisibleRumors.Effects
{
    public class MaterialOffsetEffect : IAudioEffect
    {
        // TODO
        // Split into two effects, one for regular material, one for terrain material

        [System.Serializable]
        public struct MateiralLayerOffsetDirection
        {
            // use layer OR material
            public Material material;
            public int layerID;
            public Vector2 offsetDirection;
            public float offsetSpeed;
        }

        [Space, Header("MaterialOffsetEffect")]
        [SerializeField] Terrain terrain;
        [SerializeField] List<MateiralLayerOffsetDirection> mateiralLayerOffsetDirections;
        [SerializeField] float maxSpeedBoost;
        [SerializeField, Range(0, 1)] float boostStrength; // for hand tweaking, a knob would be cool
        [SerializeField] Transform offsetFlowTarget;

        float currentOffset;

        void OnDisable()
        {
            ResetMaterials();
        }

        public override void OnEffectActivate()
        {
            currentOffset = 0f;
        }

        public override void OnEffectDeactivate()
        {
            ResetMaterials();
            currentOffset = 0f;
        }

        public override void OnEffectUpkeepTick()
        {
            float modifier = Mathf.Pow(_audioData.AmplitudeRanged, 2f);

            float offsetSpeed = modifier * maxSpeedBoost * boostStrength;

            foreach (var mat in mateiralLayerOffsetDirections)
            {
                offsetSpeed *= mat.offsetSpeed;

                if (mat.material != null)
                {
                    mat.material.mainTextureOffset += new Vector2(offsetSpeed, offsetSpeed) * mat.offsetDirection;
                }
                else
                {
                    terrain.terrainData.terrainLayers[mat.layerID].tileOffset += new Vector2(offsetSpeed, offsetSpeed) * mat.offsetDirection;
                }
            }
        }

        private void ResetMaterials()
        {
            foreach (var mat in mateiralLayerOffsetDirections)
            {
                if (mat.material != null)
                {
                    mat.material.mainTextureOffset = new Vector2(0f, 0f);
                }
            }
        }
    }
}