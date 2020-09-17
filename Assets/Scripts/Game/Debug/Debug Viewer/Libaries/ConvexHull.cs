using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConvexHull {
	
	public static List<Vector2> MakeHull(Vector2[] points) {
		List<Vector2> hull = new List<Vector2>();

		int leftMostIndex = 0;
		float xMin = float.MaxValue;

		for (int i = 0; i < points.Length; i++)
		{
			if (points[i].x < xMin) {
				xMin = points[i].x;
				leftMostIndex = i;
			}
		}

		Vector2 pointOnHull = points[leftMostIndex];
		hull.Add(pointOnHull);
		
		int x = 0;

		while (true) {
			Vector2 endpoint = points[0];
			x++;
			if (x > 1000) {
				Debug.Log("Exitting");
				Debug.Break();
				return null;
			}
			for (int i = 1; i < points.Length; i++)
			{
				if (endpoint == pointOnHull || MathUtility.SideOfLine(pointOnHull, endpoint, points[i]) == -1) {
					endpoint = points[i];
				}
			}
			pointOnHull = endpoint;
			if (pointOnHull == hull[0]) {
				break;
			}
			hull.Add(pointOnHull);
		}

		return hull;
	}

	
	
}