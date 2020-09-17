using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TriangleNet.Geometry;
using UnityEngine;

// From https://github.com/eppz/Triangle.NET
// With some modifications
public class Triangulator {

    public enum Space { XY, XZ }

    public static MeshData2D Triangulate (IEnumerable<Vector2> outline, IEnumerable<Vector2> innerPoints, IEnumerable<IEnumerable<Vector2>> holes) {
        Polygon polygon = new Polygon ();

        // Outline
        if (outline != null) {
            polygon.Add (new Contour (PointsToVertices (outline, 0)));
        }
        // Inner points
        if (innerPoints != null) {
            polygon.Points.AddRange (PointsToVertices (innerPoints, polygon.Points.Count));
        }

        // Holes
        if (holes != null) {
            foreach (var holePoints in holes) {
                polygon.Add (new Contour (PointsToVertices (holePoints, polygon.Points.Count)), true);
            }
        }
        var triangulation = polygon.Triangulate ();

        var triangles = new int[triangulation.Triangles.Count * 3];
        var points = new Vector2[polygon.Points.Count];
        for (int i = 0; i < points.Length; i++) {
            points[i] = new Vector2 ((float) polygon.Points[i].x, (float) polygon.Points[i].y);
        }

        int triangleIndex = 0;
        foreach (var tri in triangulation.Triangles) {
            for (int i = 0; i < 3; i++) {
                triangles[triangleIndex * 3 + i] = tri.GetVertex (2 - i).index;
            }
            triangleIndex++;
        }

        return new MeshData2D (points, triangles);
    }

    public class MeshData2D {
        public Vector2[] points;
        public int[] triangles;

        public MeshData2D (Vector2[] points, int[] triangles) {
            this.points = points;
            this.triangles = triangles;
        }

        public Vector3[] CreateVertices (Space space, float missingComponentValue = 0) {
            Vector3[] verts = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++) {
                if (space == Space.XY) {
                    verts[i] = new Vector3 (points[i].x, points[i].y, missingComponentValue);
                } else if (space == Space.XZ) {
                    verts[i] = new Vector3 (points[i].x, missingComponentValue, points[i].y);
                }
            }
            return verts;
        }
    }

    static IEnumerable<Vertex> PointsToVertices (IEnumerable<Vector2> points, int startI = 0) {
        var verts = new Vertex[points.Count ()];
        int i = 0;
        foreach (var p in points) {
            var vertex = new Vertex (p.x, p.y);
            vertex.index = startI + i;
            verts[i] = vertex;
            i++;
        }
        return verts;
    }

}