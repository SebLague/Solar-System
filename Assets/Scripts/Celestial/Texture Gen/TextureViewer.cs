using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TextureViewer : MonoBehaviour {

    public TextureGenerator generator;

    [Header ("Preview Settings")]
    [Range (1, 10)]
    public int tiling = 1;
    public Material previewMaterial;
    public MeshRenderer[] previewObjects;

    void Update () {
        UpdateTexture ();
    }

    public void UpdateTexture () {
        if (generator) {
            var renderTexture = generator.GenerateTexture ();
            DisplayPreview (renderTexture);
        }
    }

    void DisplayPreview (RenderTexture renderTexture) {
        if (previewMaterial == null) {
            previewMaterial = new Material (Shader.Find ("Unlit/Texture"));
        }

        previewMaterial.SetTexture ("_MainTex", renderTexture);
        previewMaterial.mainTextureScale = new Vector2 (tiling, tiling);
        if (previewObjects != null) {
            foreach (var previewObject in previewObjects) {
                if (previewObject) {
                    previewObject.sharedMaterial = previewMaterial;
                }
            }
        }
    }

    public void SaveTexture (string path) {
        if (generator) {
            var renderTexture = generator.GenerateTexture ();
            var oldRT = RenderTexture.active;

            var tex2D = new Texture2D (renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            tex2D.ReadPixels (new Rect (0, 0, renderTexture.width, renderTexture.height), 0, 0);
            tex2D.Apply ();

            System.IO.File.WriteAllBytes (System.IO.Path.Combine (path, renderTexture.name + ".png"), tex2D.EncodeToPNG ());
            RenderTexture.active = oldRT;
        }
    }

}