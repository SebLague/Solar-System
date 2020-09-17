using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomDebug {
	public static void DrawSphere (Vector3 centre, float radius, Color col) {
		int resolution = 4;
		var sphereMesh = new SphereMesh (resolution);

		for (int i = 0; i < sphereMesh.Triangles.Length; i += 3) {
			var v1 = centre + sphereMesh.Vertices[sphereMesh.Triangles[i]] * radius;
			var v2 = centre + sphereMesh.Vertices[sphereMesh.Triangles[i + 1]] * radius;
			var v3 = centre + sphereMesh.Vertices[sphereMesh.Triangles[i + 2]] * radius;
			Debug.DrawLine (v1, v2, col);
			Debug.DrawLine (v2, v3, col);
			Debug.DrawLine (v3, v1, col);
		}
	}
}