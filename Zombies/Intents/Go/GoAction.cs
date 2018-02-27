// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using UnityEngine;
using UnityEngine.AI;

using Strings = Mixspace.Lexicon.Actions.GoStrings;

namespace Mixspace.Lexicon.Actions
{
    public class GoAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            foreach (LexiconEntityMatch selectionMatch in runtimeResult.GetEntityMatches(Strings.Selection))
            {
                LexiconEntityMatch positionMatch = runtimeResult.GetEntityAfter(Strings.Position, selectionMatch);

                if (positionMatch != null)
                {
                    FocusSelection focusSelection = selectionMatch.FocusSelection;
                    FocusPosition focusPosition = positionMatch.FocusPosition;

                    if (focusSelection != null && focusPosition != null)
                    {
                        NavMeshAgent agent = focusSelection.SelectedObject.GetComponent<NavMeshAgent>();
                        NavigateToPosition(agent, focusPosition.Position);
                    }
                }
            }

            return true;
        }

        public void NavigateToPosition(NavMeshAgent agent, Vector3 position)
        {
            if (agent == null)
            {
                Debug.Log("NavigateToPosition: agent is null");
                return;
            }

            if (!agent.isOnNavMesh)
            {
                Debug.Log("NavigateToPosition: trying to fix agent");
                agent.enabled = false;
                agent.enabled = true;
            }

            Debug.Log("NavigateToPosition: " + position);
            agent.destination = position;
            agent.isStopped = false;
        }
    }
}
