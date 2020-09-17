using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization.MeshGeneration {

    public static class ArcMesh {

        const int resolution = 50;

        public static void GenerateMesh (Mesh mesh, float angle) {

            int numIncrements = (int) Mathf.Max (5, resolution * angle / 360);

            float angleIncrement = angle / (numIncrements - 1f);
            var verts = new Vector3[numIncrements + 1];
            var norms = new Vector3[numIncrements + 1];
            var tris = new int[(numIncrements - 1) * 3];
            verts[0] = Vector3.zero;
            norms[0] = Vector3.up;

            for (int i = 0; i < numIncrements; i++) {
                float currAngle = (angleIncrement * i) * Mathf.Deg2Rad;
                Vector3 pos = new Vector3 (Mathf.Sin (currAngle), 0, Mathf.Cos (currAngle));
                verts[i + 1] = pos;
                norms[i + 1] = Vector3.up;

                if (i < numIncrements - 1) {
                    tris[i * 3] = 0;
                    tris[i * 3 + 1] = i + 1;
                    tris[i * 3 + 2] = i + 2;
                }
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.normals = norms;
        }
    }
}