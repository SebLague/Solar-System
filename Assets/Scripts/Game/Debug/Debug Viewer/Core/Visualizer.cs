using System.Collections.Generic;
using UnityEngine;
using Visualization;
using Visualization.MeshGeneration;
using static Visualization.Manager;

public static class Visualizer {

    public static Color activeColour = Color.black;
    public static Style activeStyle = Style.Unlit;

    const float lineThicknessFactor = 1 / 30f;

    // --------Style Functions--------

    public static void SetColour (Color colour) {
        activeColour = colour;
    }

    public static void SetStyle (Style style) {
        activeStyle = style;
    }

    public static void SetColourAndStyle (Color colour, Style style) {
        activeColour = colour;
        activeStyle = style;
    }

    public static void DrawSphere (Vector3 centre, float radius) {
        CreateVisualElement (sphereMesh, centre, Quaternion.identity, Vector3.one * radius);
    }

    public static void DrawLine (Vector3 start, Vector3 end, float thickness) {
        Mesh mesh = CreateOrRecycleMesh ();
        CylinderMesh.GenerateMesh (mesh);
        Vector3 centre = (start + end) / 2;
        var rot = Quaternion.FromToRotation (Vector3.up, (start - end).normalized);
        Vector3 scale = new Vector3 (thickness * lineThicknessFactor, (start - end).magnitude, thickness * lineThicknessFactor);
        CreateVisualElement (mesh, centre, rot, scale);
    }

    public static void DrawRay (Vector3 start, Vector3 dir, float thickness) {
        DrawLine (start, start + dir, thickness);
    }

    public static void DrawRing (Vector3 centre, Vector3 normal, float startAngle, float angle, float innerRadius, float outerRadius) {
        Mesh mesh = CreateOrRecycleMesh ();
        RingMesh.GenerateMesh (mesh, angle, innerRadius, outerRadius);

        //float localYAngle = (startAngle - angle / 2); // centre angle
        float localYAngle = startAngle;
        var rot = Quaternion.AngleAxis (localYAngle, normal) * Quaternion.FromToRotation (Vector3.up, normal);
        CreateVisualElement (mesh, centre, rot, Vector3.one);
    }

    public static void DrawDisc (Vector3 centre, Vector3 normal, float radius) {
        DrawRing (centre, normal, 0, 360, 0, radius);
    }

    public static void DrawArc (Vector3 centre, Vector3 normal, float startAngle, float angle, float radius) {
        DrawRing (centre, normal, startAngle, angle, 0, radius);
    }

    public static void DrawPolygon2D (IEnumerable<Vector2> points, float zDepth = 0) {
        Mesh mesh = CreateOrRecycleMesh ();
        var meshData = Triangulator.Triangulate (points, null, null);

        mesh.vertices = meshData.CreateVertices (Triangulator.Space.XY, zDepth);
        mesh.triangles = meshData.triangles;
        mesh.RecalculateBounds ();
        mesh.RecalculateNormals ();
        CreateVisualElement (mesh, Vector3.zero, Quaternion.identity, Vector3.one);
    }

    /// Draw a polygon of the convex hull of given points
    public static void DrawConvexHull2D (IEnumerable<Vector2> points, float zDepth = 0) {
        var hullPoints = ConvexHull.MakeHull (new List<Vector2> (points).ToArray ());
        DrawPolygon2D (hullPoints, zDepth);
    }

    /// Draw a 2D triangle
    public static void DrawTriangle2D (Vector3 centre, float angle, float size, Vector3 normal) {
        Mesh mesh = CreateOrRecycleMesh ();
        var rot = Quaternion.AngleAxis (-angle, normal) * Quaternion.FromToRotation (Vector3.up, normal);
        mesh.vertices = new Vector3[] { Vector3.forward, Vector3.back + Vector3.right, Vector3.back - Vector3.right };
        mesh.triangles = new int[] { 0, 1, 2 };
        mesh.normals = new Vector3[] { normal, normal, normal };
        mesh.RecalculateBounds ();
        CreateVisualElement (mesh, centre, rot, Vector3.one * size);
    }

    /// Draw a 2D arrow (on xy plane)
    public static void DrawArrow2D (Vector2 start, Vector2 end, float lineWidth, float headSize, bool flatHead = true, float zDepth = 0) {
        Mesh mesh = CreateOrRecycleMesh ();

        Vector2 forward = (end - start).normalized;
        Vector2 perp = Vector3.Cross (forward, Vector3.forward);

        Vector3[] verts = new Vector3[7];

        float actualHeadSize = lineWidth * 2 + headSize;
        float headBackAmount = (flatHead) ? 0 : 0.35f;
        end -= forward * actualHeadSize;
        verts[0] = start - perp * lineWidth / 2;
        verts[1] = start + perp * lineWidth / 2;
        verts[2] = end - perp * lineWidth / 2;
        verts[3] = end + perp * lineWidth / 2;
        verts[4] = end + forward * actualHeadSize;
        verts[5] = end - forward * actualHeadSize * headBackAmount - perp * actualHeadSize / 2;
        verts[6] = end - forward * actualHeadSize * headBackAmount + perp * actualHeadSize / 2;

        mesh.vertices = verts;
        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3, 2, 5, 4, 2, 4, 3, 3, 4, 6 };
        mesh.RecalculateBounds ();

        CreateVisualElement (mesh, Vector3.forward * zDepth, Quaternion.identity, Vector3.one);
    }

    public static void DrawArrow3D (Vector3 start, Vector3 end, float lineWidth, float headWidth) {
        float length = (end - start).magnitude;
        Mesh mesh = CreateOrRecycleMesh ();
        ArrowMesh3D.GenerateMesh (mesh, length, lineWidth * lineThicknessFactor, headWidth * lineThicknessFactor);
        Vector3 centre = (start + end) / 2;
        var rot = Quaternion.FromToRotation (Vector3.up, (end - start).normalized);
        Vector3 scale = Vector3.one;
        CreateVisualElement (mesh, start, rot, scale);
    }

}