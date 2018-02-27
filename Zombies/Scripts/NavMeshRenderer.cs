// Copyright (c) 2018 Mixspace Technologies, LLC. All rights reserved.

using UnityEngine;
using UnityEngine.AI;

namespace Mixspace.Lexicon.Samples
{
    public class NavMeshRenderer : MonoBehaviour
    {
        void OnEnable()
        {
            UpdateMesh();
        }

        public void UpdateMesh()
        {
            Debug.Log("NavMeshRenderer UpdateMesh");

            MeshFilter meshFilter = GetComponent<MeshFilter>();

            if (meshFilter != null)
            {
                NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

                Mesh mesh = new Mesh();
                mesh.vertices = triangulation.vertices;
                mesh.triangles = triangulation.indices;

                Debug.Log("NavMesh triangle count: " + triangulation.indices.Length);

                meshFilter.mesh = mesh;
            }
        }
    }
}
