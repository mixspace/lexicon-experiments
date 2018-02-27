// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.DimensionsStrings;

namespace Mixspace.Lexicon.Actions
{
    public class DimensionsAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            List<LexiconEntityMatch> selections = runtimeResult.GetEntityMatches(Strings.Selection);

            foreach (LexiconEntityMatch selection in selections)
            {
                if (selection.FocusSelection != null)
                {
                    GameObject selectedObject = selection.FocusSelection.SelectedObject;

                    List<LexiconEntityMatch> numbers = runtimeResult.GetEntitiesAfter(Strings.SysNumber, selection);

                    foreach (LexiconEntityMatch number in numbers)
                    {
                        LexiconEntityMatch unit = runtimeResult.GetEntityAfter(Strings.Unit, number);
                        LexiconEntityMatch dimension = runtimeResult.GetEntityAfter(Strings.Dimension, number);

                        if (unit != null && dimension != null)
                        {
                            //bool uniform = (numbers.Count == 1);
                            bool uniform = true;
                            if (selectedObject.name == "Cube" || selectedObject.name == "Cylinder")
                            {
                                uniform = false;
                            }
                            Resize(selectedObject, number.SystemValue.FloatValue, unit.EntityValue.ValueName, dimension.EntityValue.ValueName, uniform);
                        }
                    }
                }
            }
            
            return true;
        }

        private void Resize(GameObject target, float size, string unit, string dimension, bool uniform)
        {
            Debug.Log("Resize: " + target.name + ", size: " + size + ", unit: " + unit + ", dimension: " + dimension + ", uniform: " + uniform);

            Renderer renderer = target.GetComponentInChildren<Renderer>();
            if (renderer != null)
            {
                Vector3 currentSize = renderer.bounds.size;
                Vector3 currentScale = target.transform.localScale;

                float sizeMeters = size;

                switch (unit)
                {
                    case Strings.UnitValues.Centimeters:
                        sizeMeters = size * 0.01f;
                        break;
                    case Strings.UnitValues.Feet:
                        sizeMeters = size * 0.3048f;
                        break;
                    case Strings.UnitValues.Inches:
                        sizeMeters = size * 0.0254f;
                        break;
                }

                float scale;
                switch (dimension)
                {
                    case Strings.DimensionValues.Width:
                        scale = sizeMeters / currentSize.x;
                        if (uniform)
                        {
                            target.transform.localScale = new Vector3(scale * currentScale.x, scale * currentScale.y, scale * currentScale.z);
                        }
                        else
                        {
                            target.transform.localScale = new Vector3(scale * currentScale.x, currentScale.y, currentScale.z);
                        }
                        break;
                    case Strings.DimensionValues.Height:
                        scale = sizeMeters / currentSize.y;
                        if (uniform)
                        {
                            target.transform.localScale = new Vector3(scale * currentScale.x, scale * currentScale.y, scale * currentScale.z);
                        }
                        else
                        {
                            target.transform.localScale = new Vector3(currentScale.x, scale * currentScale.y, currentScale.z);
                        }
                        break;
                    case Strings.DimensionValues.Depth:
                        scale = sizeMeters / currentSize.z;
                        if (uniform)
                        {
                            target.transform.localScale = new Vector3(scale * currentScale.x, scale * currentScale.y, scale * currentScale.z);
                        }
                        else
                        {
                            target.transform.localScale = new Vector3(currentScale.x, currentScale.y, scale * currentScale.z);
                        }
                        break;
                }
            }
        }
    }
}
