using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization.MeshGeneration {
    public static class RingMesh {

        const int resolution = 50;

        public static void GenerateMesh (Mesh mesh, float angle, float innerRadius, float outerRadius) {

            // Validate input:
            angle = Mathf.Clamp (angle, -360, 360);
            innerRadius = Mathf.Max (0, innerRadius);
            outerRadius = Mathf.Max (0, outerRadius);
            if (outerRadius < innerRadius) {
                float temp = outerRadius;
                outerRadius = innerRadius;
                innerRadius = temp;
            }

            int numIncrements = (int) Mathf.Max (5, resolution * Mathf.Abs (angle) / 360);

            float angleIncrement = angle / (numIncrements - 1f);
            var verts = new Vector3[numIncrements * 2];
            var norms = new Vector3[numIncrements * 2];
            var tris = new int[(numIncrements - 1) * 2 * 3];

            for (int i = 0; i < numIncrements; i++) {
                float currAngle = (angleIncrement * i) * Mathf.Deg2Rad;
                Vector3 dir = new Vector3 (Mathf.Sin (currAngle), 0, Mathf.Cos (currAngle));
                Vector3 pos = dir * outerRadius;
                Vector3 posInner = dir * innerRadius;
                // If angle < 0 then reverse verts so that triangles still wind the right way
                verts[i * 2] = (angle > 0) ? posInner : pos;
                verts[i * 2 + 1] = (angle > 0) ? pos : posInner;
                norms[i * 2] = Vector3.up;
                norms[i * 2 + 1] = Vector3.up;

                if (i < numIncrements - 1) {
                    tris[i * 6] = i * 2;
                    tris[i * 6 + 1] = i * 2 + 1;
                    tris[i * 6 + 2] = i * 2 + 2;

                    tris[i * 6 + 3] = i * 2 + 2;
                    tris[i * 6 + 4] = i * 2 + 1;
                    tris[i * 6 + 5] = i * 2 + 3;
                }
            }

            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.normals = norms;
        }
    }
}