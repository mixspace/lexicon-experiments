// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;

using Strings = Mixspace.Lexicon.Actions.FollowStrings;

namespace Mixspace.Lexicon.Actions
{
    public class FollowAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            List<LexiconEntityMatch> matches = runtimeResult.GetEntityMatches(Strings.Selection);

            for (int i = 0; i < matches.Count; i += 2)
            {
                if ((i + 1) >= matches.Count)
                {
                    break;
                }

                LexiconEntityMatch selectionEntity = matches[i];
                LexiconEntityMatch targetEntity = matches[i + 1];

                FocusSelection selection = selectionEntity.FocusSelection;
                FocusSelection target = targetEntity.FocusSelection;

                if (selection != null && target != null)
                {
                    EnemyNavigation navigation = selection.SelectedObject.GetComponent<EnemyNavigation>();
                    if (navigation != null)
                    {
                        navigation.target = target.SelectedObject;
                    }
                }
            }

            return true;
        }
    }
}
