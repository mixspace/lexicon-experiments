// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.StopDebuggingStrings;

namespace Mixspace.Lexicon.Actions
{
    public class StopDebuggingAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            LexiconEntityMatch match = runtimeResult.GetEntityMatch(Strings.Debug);

            GameObject prefab = match.EntityValue.GetBinding<GameObject>();

            if (prefab != null)
            {
                LexiconPrefabManager.Instance.DestroyInstance(prefab);
            }

            return false;
        }
    }
}
