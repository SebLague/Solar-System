using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization.MeshGeneration {
    public static class ArrowMesh3D {

        const int resolution = 30;

        public static void GenerateMesh (Mesh mesh, float length, float lineRadius, float headRadius) {

            float headLength = Mathf.Min (length / 2, headRadius * 2.5f);

            var bottomVerts = new List<Vector3> ();
            var bottomTris = new List<int> ();
            var topVerts = new List<Vector3> ();

            var sideVerts = new List<Vector3> ();
            var sideTris = new List<int> ();

            var headBaseVerts = new List<Vector3> ();
            var headBaseTris = new List<int> ();
            var headConeVerts = new List<Vector3> ();
            var headConeTris = new List<int> ();

            // Top/bottom face
            Vector3 bottomCentre = Vector3.zero;
            Vector3 topCentre = Vector3.up * (length - headLength);

            for (int i = 0; i < resolution; i++) {
                float angle = i / (float) (resolution) * Mathf.PI * 2;
                Vector3 offsetDir = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle));
                bottomVerts.Add (bottomCentre + offsetDir * lineRadius);
                bottomTris.AddRange (new int[] { resolution, (i + 1) % resolution, i % resolution });
                topVerts.Add (topCentre + offsetDir * lineRadius);

                // Create circle as base for arrow head cone
                headBaseVerts.Add (topCentre + offsetDir * headRadius);
                headBaseTris.AddRange (new int[] { 0, (i + 1) % resolution, i % resolution });

                headConeVerts.Add (topCentre + offsetDir * headRadius);
                headConeTris.AddRange (new int[] { resolution, i, (i + 1) % resolution });
            }
            headConeVerts.Add (Vector3.up * length);
            headBaseVerts.Add (topCentre);
            sideVerts.AddRange (bottomVerts);
            sideVerts.AddRange (topVerts);
            bottomVerts.Add (bottomCentre);

            // Sides
            for (int i = 0; i < resolution; i++) {
                sideTris.Add (i);
                sideTris.Add ((i + 1) % resolution + resolution);
                sideTris.Add (i + resolution);

                sideTris.Add (i);
                sideTris.Add ((i + 1) % resolution);
                sideTris.Add ((i + 1) % resolution + resolution);
            }

            var allVertLists = new List<Vector3>[] { bottomVerts, sideVerts, headBaseVerts, headConeVerts };
            var allTriLists = new List<int>[] { bottomTris, sideTris, headBaseTris, headConeTris };
            MeshUtility.MeshFromMultipleSources (mesh, allVertLists, allTriLists);
        }

    }
}