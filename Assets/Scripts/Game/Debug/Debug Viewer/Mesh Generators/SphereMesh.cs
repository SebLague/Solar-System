using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization.MeshGeneration {
    public static class SphereMesh {

        const int resolution = 20;
        static Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        public static void GenerateMesh (Mesh mesh) {
            List<Vector3> vertices = new List<Vector3> ();
            List<Vector3> normals = new List<Vector3> ();
            List<int> triangles = new List<int> ();

            for (int i = 0; i < 6; i++) {
                Vector3 localUp = directions[i];
                var faceData = ConstructFace (localUp, resolution);

                int numVerts = vertices.Count;
                vertices.AddRange (faceData.vertices);
                normals.AddRange (faceData.normals);

                for (int j = 0; j < faceData.triangles.Length; j++) {
                    triangles.Add (faceData.triangles[j] + numVerts);
                }
            }

            mesh.SetVertices (vertices);
            mesh.SetNormals (normals);
            mesh.SetTriangles (triangles, 0);
        }

        static FaceData ConstructFace (Vector3 localUp, int resolution) {
            Vector3 axisA = new Vector3 (localUp.y, localUp.z, localUp.x);
            Vector3 axisB = Vector3.Cross (localUp, axisA);

            Vector3[] vertices = new Vector3[resolution * resolution];
            Vector3[] normals = new Vector3[resolution * resolution];
            int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];
            int triIndex = 0;

            for (int y = 0; y < resolution; y++) {
                for (int x = 0; x < resolution; x++) {
                    int i = x + y * resolution;
                    Vector2 percent = new Vector2 (x, y) / (resolution - 1);
                    Vector3 pointOnUnitCube = localUp + (percent.x - .5f) * 2 * axisA + (percent.y - .5f) * 2 * axisB;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                    vertices[i] = pointOnUnitSphere;
                    normals[i] = pointOnUnitSphere;

                    if (x != resolution - 1 && y != resolution - 1) {
                        triangles[triIndex] = i;
                        triangles[triIndex + 1] = i + resolution + 1;
                        triangles[triIndex + 2] = i + resolution;

                        triangles[triIndex + 3] = i;
                        triangles[triIndex + 4] = i + 1;
                        triangles[triIndex + 5] = i + resolution + 1;
                        triIndex += 6;
                    }
                }
            }

            return new FaceData () { triangles = triangles, vertices = vertices, normals = normals };
        }

        public class FaceData {
            public int[] triangles;
            public Vector3[] vertices;
            public Vector3[] normals;
        }
    }
}