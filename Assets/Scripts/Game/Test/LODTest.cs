using System.Collections;
using UnityEngine;

public class LODTest : MonoBehaviour {
    public LODGroup group;
    public float[] vals;
    public bool useVals;

    void Start () {
        // Programmatically create a LOD group and add LOD levels.
        // Create a GUI that allows for forcing a specific LOD level.
        group = gameObject.AddComponent<LODGroup> ();

        // Add 4 LOD levels
        LOD[] lods = new LOD[4];
        for (int i = 0; i < 4; i++) {
            PrimitiveType primType = PrimitiveType.Cube;
            switch (i) {
                case 1:
                    primType = PrimitiveType.Capsule;
                    break;
                case 2:
                    primType = PrimitiveType.Sphere;
                    break;
                case 3:
                    primType = PrimitiveType.Cylinder;
                    break;
            }
            GameObject go = GameObject.CreatePrimitive (primType);
            go.transform.parent = gameObject.transform;
            Renderer[] renderers = new Renderer[1];
            renderers[0] = go.GetComponent<Renderer> ();
            float v = 1.0F / (i + 1);
            lods[i] = new LOD (v, renderers);
            Debug.Log (i + " screenrelative transition height: " + v);
        }
        group.SetLODs (lods);
        group.RecalculateBounds ();
    }

    void Update () {

    }

    void OnGUI () {
        if (GUILayout.Button ("Enable / Disable"))
            group.enabled = !group.enabled;

        if (GUILayout.Button ("Default"))
            group.ForceLOD (-1);

        if (GUILayout.Button ("Force 0"))
            group.ForceLOD (0);

        if (GUILayout.Button ("Force 1"))
            group.ForceLOD (1);

        if (GUILayout.Button ("Force 2"))
            group.ForceLOD (2);

        if (GUILayout.Button ("Force 3"))
            group.ForceLOD (3);

        if (GUILayout.Button ("Force 4"))
            group.ForceLOD (4);

        if (GUILayout.Button ("Force 5"))
            group.ForceLOD (5);

        if (GUILayout.Button ("Force 6"))
            group.ForceLOD (6);
    }
}