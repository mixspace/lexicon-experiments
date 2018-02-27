// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.StartDebuggingStrings;

namespace Mixspace.Lexicon.Actions
{
    public class StartDebuggingAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            LexiconEntityMatch match = runtimeResult.GetEntityMatch(Strings.Debug);

            GameObject prefab = match.EntityValue.GetBinding<GameObject>();

            if (prefab != null)
            {
                LexiconPrefabManager.Instance.FindOrCreate(prefab);
            }
            
            return true;
        }
    }
}
