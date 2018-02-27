// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using System.Collections.Generic;
using Mixspace.Lexicon;
using UnityEngine;

namespace Mixspace.Lexicon.Samples
{
    public class WordAlignmentDebug : MonoBehaviour
    {
        public Color labelColor = Color.white;
        public Shader markerShader;
        public Font labelFont;
        public int labelFontSize = 32;
        public float labelScale = 0.02f;
        public float markerScale = 0.02f;

        private LexiconFocusManager focusManager;
        private Camera mainCamera;

        private List<GameObject> labels = new List<GameObject>();
        private List<GameObject> markers = new List<GameObject>();

        private Material markerMaterial;

        void OnEnable()
        {
            LexiconRuntime.OnSpeechToTextResults += OnSpeechToTextResult;
        }

        void OnDisable()
        {
            LexiconRuntime.OnSpeechToTextResults -= OnSpeechToTextResult;
        }

        void Start()
        {
            focusManager = LexiconFocusManager.Instance;
            mainCamera = Camera.main;

            markerMaterial = new Material(markerShader);
            markerMaterial.color = labelColor;
        }

        void LateUpdate()
        {
            if (mainCamera != null)
            {
                foreach (GameObject labelObject in labels)
                {
                    labelObject.transform.rotation = Quaternion.LookRotation(labelObject.transform.position - mainCamera.transform.position);
                }
            }
        }

        void OnSpeechToTextResult(LexiconSpeechResult speechResult)
        {
            foreach (GameObject labelObject in labels)
            {
                Destroy(labelObject);
            }
            labels.Clear();

            foreach (GameObject markerObject in markers)
            {
                Destroy(markerObject);
            }
            markers.Clear();

            foreach (LexiconSpeechResult.WordResult wordResult in speechResult.WordResults)
            {
                FocusPosition focusPosition = focusManager.GetFocusData<FocusPosition>(wordResult.RealtimeStart);
                //DwellPosition dwellPosition = focusManager.GetFocusData<DwellPosition>(wordResult.realtimeStart);
                if (focusPosition != null)
                {
                    GameObject labelObject = new GameObject("WordAlignmentLabel");
                    labelObject.transform.position = focusPosition.Position;
                    labelObject.transform.localScale = new Vector3(labelScale, labelScale, labelScale);
                    labelObject.transform.parent = this.transform;
                    TextMesh textMesh = labelObject.AddComponent<TextMesh>();
                    textMesh.text = wordResult.Word;
                    textMesh.color = labelColor;
                    textMesh.font = labelFont;
                    textMesh.fontSize = labelFontSize;
                    textMesh.GetComponent<Renderer>().sharedMaterial = labelFont.material;
                    labels.Add(labelObject);

                    GameObject markerObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    markerObject.transform.position = focusPosition.Position;
                    markerObject.transform.localScale = new Vector3(markerScale, markerScale, markerScale);
                    markerObject.transform.parent = this.transform;
                    markerObject.GetComponent<Renderer>().material = markerMaterial;
                    markerObject.GetComponent<Collider>().enabled = false;
                    markers.Add(markerObject);
                }
            }
        }
    }
}
