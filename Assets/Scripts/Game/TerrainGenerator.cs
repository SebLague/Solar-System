using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TerrainGenerator : MonoBehaviour {

    public int terrainResolution = 1;
    public Material material;
    Mesh mesh;

    bool settingsChanged;

    void Update () {
        if (settingsChanged) {
            settingsChanged = false;
            if (mesh == null) {
                mesh = new Mesh ();
            }
            else {
                mesh.Clear();
            }

            IcoSphereGenerator s = new IcoSphereGenerator ();
            s.Generate (terrainResolution);
            mesh.vertices = s.Vertices;
            mesh.triangles = s.Triangles;
            mesh.RecalculateBounds ();
            mesh.RecalculateNormals ();

            var g = GetOrCreateMeshObject ("Mesh", mesh, material);
            if (!g.GetComponent<MeshCollider> ()) {
                g.AddComponent<MeshCollider> ();
            }
            g.GetComponent<MeshCollider> ().sharedMesh = mesh;
        }
    }

    GameObject GetOrCreateMeshObject (string name, Mesh mesh, Material material) {
        // Find/create object
        var child = transform.Find (name);
        if (!child) {
            child = new GameObject (name).transform;
            child.parent = transform;
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            child.gameObject.layer = gameObject.layer;
        }

        // Add mesh components
        MeshFilter filter;
        if (!child.TryGetComponent<MeshFilter> (out filter)) {
            filter = child.gameObject.AddComponent<MeshFilter> ();
        }
        filter.sharedMesh = mesh;

        MeshRenderer renderer;
        if (!child.TryGetComponent<MeshRenderer> (out renderer)) {
            renderer = child.gameObject.AddComponent<MeshRenderer> ();
        }
        renderer.sharedMaterial = material;

        return child.gameObject;
    }

    void OnValidate () {
        settingsChanged = true;
    }
}