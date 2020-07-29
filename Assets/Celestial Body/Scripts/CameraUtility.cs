using UnityEngine;

public static class CameraUtility {
    static readonly Vector3[] cubeCornerOffsets = {
        new Vector3 (1, 1, 1),
        new Vector3 (-1, 1, 1),
        new Vector3 (-1, -1, 1),
        new Vector3 (-1, -1, -1),
        new Vector3 (-1, 1, -1),
        new Vector3 (1, -1, -1),
        new Vector3 (1, 1, -1),
        new Vector3 (1, -1, 1),
    };

    // http://wiki.unity3d.com/index.php/IsVisibleFrom
    public static bool VisibleFromCamera (Renderer renderer, Camera camera) {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes (camera);
        return GeometryUtility.TestPlanesAABB (frustumPlanes, renderer.bounds);
    }

    public static bool BoundsOverlap (MeshFilter nearObject, MeshFilter farObject, Camera camera) {

        var near = GetScreenRectFromBounds (nearObject, camera);
        var far = GetScreenRectFromBounds (farObject, camera);

        // ensure far object is indeed further away than near object
        if (far.zMax > near.zMin) {
            // Doesn't overlap on x axis
            if (far.xMax < near.xMin || far.xMin > near.xMax) {
                return false;
            }
            // Doesn't overlap on y axis
            if (far.yMax < near.yMin || far.yMin > near.yMax) {
                return false;
            }
            // Overlaps
            return true;
        }
        return false;
    }

    public static float GetScreenHeight (Vector3 a, Vector3 b, Camera camera) {
        Debug.DrawLine(a,b,Color.red);
        Vector3 viewA = camera.WorldToViewportPoint (a);
        Vector3 viewB = camera.WorldToViewportPoint (b);
        float dst = Mathf.Abs (viewA.y - viewB.y);
        Debug.Log ("A: " + viewA.y + "  B: " + viewB.y + "  " + dst);
        return dst;
    }

    // With thanks to http://www.turiyaware.com/a-solution-to-unitys-camera-worldtoscreenpoint-causing-ui-elements-to-display-when-object-is-behind-the-camera/
    public static MinMax3D GetScreenRectFromBounds (MeshFilter meshFilter, Camera camera) {
        MinMax3D minMax = new MinMax3D (float.MaxValue, float.MinValue);

        Vector3[] screenBoundsExtents = new Vector3[8];
        var localBounds = meshFilter.sharedMesh.bounds;
        bool anyPointIsInFrontOfCamera = false;

        for (int i = 0; i < 8; i++) {
            Vector3 localSpaceCorner = localBounds.center + Vector3.Scale (localBounds.extents, cubeCornerOffsets[i]);
            Vector3 worldSpaceCorner = meshFilter.transform.TransformPoint (localSpaceCorner);
            Vector3 viewportSpaceCorner = camera.WorldToViewportPoint (worldSpaceCorner);

            if (viewportSpaceCorner.z > 0) {
                anyPointIsInFrontOfCamera = true;
            } else {
                // If point is behind camera, it gets flipped to the opposite side
                // So clamp to opposite edge to correct for this
                viewportSpaceCorner.x = (viewportSpaceCorner.x <= 0.5f) ? 1 : 0;
                viewportSpaceCorner.y = (viewportSpaceCorner.y <= 0.5f) ? 1 : 0;
            }

            // Update bounds with new corner point
            minMax.AddPoint (viewportSpaceCorner);
        }

        // All points are behind camera so just return empty bounds
        if (!anyPointIsInFrontOfCamera) {
            return new MinMax3D ();
        }

        return minMax;
    }

    [System.Serializable]
    public struct MinMax3D {
        public float xMin;
        public float xMax;
        public float yMin;
        public float yMax;
        public float zMin;
        public float zMax;

        public MinMax3D (float min, float max) {
            this.xMin = min;
            this.xMax = max;
            this.yMin = min;
            this.yMax = max;
            this.zMin = min;
            this.zMax = max;
        }

        public void AddPoint (Vector3 point) {
            xMin = Mathf.Min (xMin, point.x);
            xMax = Mathf.Max (xMax, point.x);
            yMin = Mathf.Min (yMin, point.y);
            yMax = Mathf.Max (yMax, point.y);
            zMin = Mathf.Min (zMin, point.z);
            zMax = Mathf.Max (zMax, point.z);
        }
    }

}