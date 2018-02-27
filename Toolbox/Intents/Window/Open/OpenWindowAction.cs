// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.OpenWindowStrings;

namespace Mixspace.Lexicon.Actions
{
    public class OpenWindowAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            List<LexiconEntityMatch> windows = runtimeResult.GetEntityMatches(Strings.Window);

            foreach (LexiconEntityMatch window in windows)
            {
                GameObject prefab = window.EntityValue.GetBinding<GameObject>();

                if (prefab != null)
                {
                    GameObject instance = LexiconPrefabManager.Instance.FindOrCreate(prefab);

                    FocusPosition focusPosition = window.FocusPosition;
                    LexiconEntityMatch positionMatch = runtimeResult.GetEntityAfter(Strings.Position, window);
                    if (positionMatch != null)
                    {
                        focusPosition = positionMatch.FocusPosition;
                    }

                    if (focusPosition != null)
                    {
                        Vector3 normal = -focusPosition.Normal;
                        float angle = Vector3.Angle(-Vector3.forward, focusPosition.Normal);
                        if (angle < 30)
                        {
                            Debug.Log("Window on forward wall");
                            normal = Vector3.forward;
                        }
                        angle = Vector3.Angle(Vector3.forward, focusPosition.Normal);
                        if (angle < 30)
                        {
                            Debug.Log("Window on backward wall");
                            normal = -Vector3.forward;
                        }
                        angle = Vector3.Angle(-Vector3.right, focusPosition.Normal);
                        if (angle < 30)
                        {
                            Debug.Log("Window on right wall");
                            normal = Vector3.right;
                        }
                        angle = Vector3.Angle(Vector3.right, focusPosition.Normal);
                        if (angle < 30)
                        {
                            Debug.Log("Window on left wall");
                            normal = -Vector3.right;
                        }

                        instance.transform.position = focusPosition.Position - normal * 0.05f;
                        instance.transform.forward = -focusPosition.Normal;
                    }
                }
            }

            return true;
        }
    }
}
