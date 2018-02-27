// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using UnityEngine;
using UnityEngine.AI;

using Strings = Mixspace.Lexicon.Actions.UpdateNavmeshStrings;

namespace Mixspace.Lexicon.Actions
{
    public class UpdateNavmeshAction : LexiconAction
    {
        public GameObject debugNavigationPrefab;

        public NavMeshAgent agent;

        public override bool Process(LexiconRuntimeResult runtimeResult)
        {
            Debug.Log("Building Nav Mesh");

            NavMeshSurface surface = FindObjectOfType<NavMeshSurface>();

            if (surface == null)
            {
                GameObject navagation = new GameObject("Navigation");
                surface = navagation.AddComponent<NavMeshSurface>();
                surface.agentTypeID = agent.agentTypeID;
            }

            if (surface != null)
            {
                surface.BuildNavMesh();

                bool existed = LexiconPrefabManager.Instance.Exists(debugNavigationPrefab);

                GameObject debugNavigation = LexiconPrefabManager.Instance.FindOrCreate(debugNavigationPrefab);

                if (existed)
                {
                    // Only update the mesh if it already existed.
                    debugNavigation.GetComponent<Samples.NavMeshRenderer>().UpdateMesh();
                }
            }

            return true;
        }
    }
}
