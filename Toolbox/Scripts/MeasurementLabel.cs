using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mixspace.Lexicon.Samples
{
    public class MeasurementLabel : MonoBehaviour
    {
        private Camera mainCamera;

        void Start()
        {
            mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (mainCamera != null)
            {
                transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
            }
        }
    }
}
