// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.CloseWindowStrings;

namespace Mixspace.Lexicon.Actions
{
    public class CloseWindowAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            List<LexiconEntityMatch> windows = runtimeResult.GetEntityMatches(Strings.Window);

            foreach (LexiconEntityMatch window in windows)
            {
                GameObject prefab = window.EntityValue.GetBinding<GameObject>();

                if (prefab != null)
                {
                    LexiconPrefabManager.Instance.DestroyInstance(prefab);
                }
            }
            
            return false;
        }
    }
}
