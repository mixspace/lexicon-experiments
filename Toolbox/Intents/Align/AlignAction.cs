// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.AlignStrings;

namespace Mixspace.Lexicon.Actions
{
    public class AlignAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            LexiconEntityMatch alignment = runtimeResult.GetEntityMatch(Strings.Alignment);

            List<LexiconEntityMatch> selections = runtimeResult.GetEntityMatches(Strings.Selection);

            List<GameObject> gameObjects = new List<GameObject>();

            foreach (LexiconEntityMatch selection in selections)
            {
                if (selection.FocusSelection != null)
                {
                    gameObjects.Add(selection.FocusSelection.SelectedObject);
                }
            }

            if (gameObjects.Count > 1)
            {
                GameObject anchorObject = gameObjects[0];                 Bounds anchorBounds = anchorObject.GetComponentInChildren<Renderer>().bounds;

                for (int i = 1; i < gameObjects.Count; i++)
                {
                    GameObject otherObject = gameObjects[i];                     Bounds otherBounds = otherObject.GetComponentInChildren<Renderer>().bounds;                     Vector3 otherPosition = otherObject.transform.position;                     float yOffset = 0;
                    float xOffset = 0;

                    switch (alignment.EntityValue.ValueName)
                    {
                        case Strings.AlignmentValues.Top:
                            yOffset = anchorBounds.max.y - otherBounds.max.y;
                            break;
                        case Strings.AlignmentValues.Bottom:
                            yOffset = anchorBounds.min.y - otherBounds.min.y;
                            break;
                        case Strings.AlignmentValues.Center:
                            yOffset = anchorBounds.center.y - otherBounds.center.y;
                            break;
                        case Strings.AlignmentValues.Right:
                            xOffset = anchorBounds.max.x - otherBounds.max.x;
                            break;
                        case Strings.AlignmentValues.Left:
                            xOffset = anchorBounds.min.x - otherBounds.min.x;
                            break;
                    }

                    otherObject.transform.position = new Vector3(otherPosition.x + xOffset, otherPosition.y + yOffset, otherPosition.z);
                }

                return true;
            }

            return false;
        }
    }
}
