using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization.MeshGeneration {
    public static class CylinderMesh {

        const int resolution = 20;

        public static void GenerateMesh (Mesh mesh) {

            float radius = .5f;

            var bottomVerts = new List<Vector3> ();
            var bottomTris = new List<int> ();

            var topVerts = new List<Vector3> ();
            var topTris = new List<int> ();

            var sideVerts = new List<Vector3> ();
            var sideTris = new List<int> ();

            // Top/bottom face
            Vector3 bottomCentre = Vector3.down * .5f;
            Vector3 topCentre = Vector3.up * .5f;

            for (int i = 0; i < resolution; i++) {
                float angle = i / (float) (resolution) * Mathf.PI * 2;
                Vector3 offset = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle)) * radius;
                bottomVerts.Add (bottomCentre + offset);
                bottomTris.AddRange (new int[] { resolution, (i + 1) % resolution, i % resolution });

                topVerts.Add (topCentre + offset);
                topTris.AddRange (new int[] { resolution, i % resolution, (i + 1) % resolution });
            }

            sideVerts.AddRange (bottomVerts);
            sideVerts.AddRange (topVerts);

            bottomVerts.Add (bottomCentre);
            topVerts.Add (topCentre);

            // Sides
            for (int i = 0; i < resolution; i++) {
                sideTris.Add (i);
                sideTris.Add ((i + 1) % resolution + resolution);
                sideTris.Add (i + resolution);

                sideTris.Add (i);
                sideTris.Add ((i + 1) % resolution);
                sideTris.Add ((i + 1) % resolution + resolution);
            }
            var allVertLists = new List<Vector3>[] { topVerts, bottomVerts, sideVerts };
            var allTriLists = new List<int>[] { topTris, bottomTris, sideTris };
            MeshUtility.MeshFromMultipleSources (mesh, allVertLists, allTriLists);
        }

    }
}