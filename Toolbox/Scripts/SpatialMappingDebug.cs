// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using UnityEngine;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#else
using UnityEngine.VR.WSA;
#endif

namespace Mixspace.Lexicon.Samples
{
    public class SpatialMappingDebug : MonoBehaviour
    {

#if UNITY_WSA
        SpatialMappingRenderer spatialMappingRenderer;

        void OnEnable()
        {
            spatialMappingRenderer = FindObjectOfType<SpatialMappingRenderer>();

            if (spatialMappingRenderer == null)
            {
                gameObject.AddComponent<SpatialMappingRenderer>();
            }

            spatialMappingRenderer.enabled = true;
        }

        void OnDisable()
        {
            spatialMappingRenderer.enabled = false;
        }
#endif

    }
}
