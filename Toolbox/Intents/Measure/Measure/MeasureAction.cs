using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.MeasureStrings;

namespace Mixspace.Lexicon.Actions
{
    public class MeasureAction : LexiconAction
    {
        public Color labelColor = Color.white;
        public Font labelFont;
        public int labelFontSize = 32;
        public float labelScale = 0.02f;
        public bool drawLine = true;

        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            List<LexiconEntityMatch> positions = runtimeResult.GetEntityMatches(Strings.Position);

            if (positions.Count != 2)
            {
                return false;
            }

            if (positions[0].FocusPosition != null && positions[1].FocusPosition != null)
            {
                Vector3 position1 = positions[0].FocusPosition.Position;
                Vector3 position2 = positions[1].FocusPosition.Position;
                Vector3 midpoint = (position1 + position2) / 2.0f;

                float distance = Vector3.Distance(position1, position2);
                string unitString = "m";

                LexiconEntityMatch unit = runtimeResult.GetEntityMatch(Strings.Unit);
                if (unit != null)
                {
                    switch (unit.EntityValue.ValueName)
                    {
                        case Strings.UnitValues.Centimeters:
                            distance *= 100;
                            unitString = "cm";
                            break;
                        case Strings.UnitValues.Feet:
                            distance *= 3.28084f;
                            unitString = "ft";
                            break;
                        case Strings.UnitValues.Inches:
                            distance *= 39.3701f;
                            unitString = "in";
                            break;
                    }
                }
                
                GameObject labelObject = new GameObject("MeasurementLabel");
                labelObject.transform.position = midpoint;
                labelObject.transform.localScale = new Vector3(labelScale, labelScale, labelScale);
                TextMesh textMesh = labelObject.AddComponent<TextMesh>();
                textMesh.text = string.Format("{0:f2} {1}", distance, unitString);
                textMesh.color = labelColor;
                textMesh.font = labelFont;
                textMesh.fontSize = labelFontSize;
                textMesh.GetComponent<Renderer>().sharedMaterial = labelFont.material;
                labelObject.AddComponent<Samples.MeasurementLabel>();

                if (drawLine)
                {
                    LineRenderer lineRenderer = labelObject.AddComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, position1);
                    lineRenderer.SetPosition(1, position2);
                    lineRenderer.startWidth = 0.01f;
                    lineRenderer.endWidth = 0.01f;
                    lineRenderer.startColor = labelColor;
                    lineRenderer.endColor = labelColor;
                }

                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    labelObject.transform.rotation = Quaternion.LookRotation(labelObject.transform.position - mainCamera.transform.position);
                }
            }

            return true;
        }
    }
}
