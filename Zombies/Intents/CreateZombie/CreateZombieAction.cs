// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using UnityEngine;
using UnityEngine.AI;

using Strings = Mixspace.Lexicon.Actions.CreateZombieStrings;

namespace Mixspace.Lexicon.Actions
{
    public class CreateZombieAction : LexiconAction
    {
        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            Camera mainCamera = Camera.main;

            foreach (LexiconEntityMatch modelMatch in runtimeResult.GetEntityMatches(Strings.Zombie))
            {
                GameObject model = Instantiate(modelMatch.EntityValue.GetBinding<GameObject>());
                model.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                model.AddComponent<LexiconSelectable>();

                Vector3 position = new Vector3(0, 0, 0);
                LexiconEntityMatch positionMatch = runtimeResult.GetEntityAfter(Strings.Position, modelMatch);
                if (positionMatch != null && positionMatch.FocusPosition != null)
                {
                    position = positionMatch.FocusPosition.Position;
                }
                else if (modelMatch.FocusPosition != null)
                {
                    position = modelMatch.FocusPosition.Position;
                }
                else
                {
                    if (mainCamera != null)
                    {
                        position = mainCamera.transform.position + mainCamera.transform.forward * 2.0f;
                    }
                }

                NavMeshAgent agent = model.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    agent.Warp(position);
                }
                else
                {
                    model.transform.position = position;
                }

                // Face the user
                if (mainCamera != null)
                {
                    Vector3 forwardVector = mainCamera.transform.position - model.transform.position;
                    model.transform.forward = new Vector3(forwardVector.x, model.transform.position.y, forwardVector.z);
                }

#if NETFX_CORE
                // Turn off point light on HoloLens
                model.GetComponentInChildren<Light>().enabled = false;
#endif
            }

            return true;
        }
    }
}
