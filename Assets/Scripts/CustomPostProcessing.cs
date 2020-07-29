using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class CustomPostProcessing : MonoBehaviour {

	BodyEffectManager effectManager;
	List<RenderTexture> renderSources = new List<RenderTexture> ();
	List<RenderTexture> renderTargets = new List<RenderTexture> ();
	List<Material> materials;
	Material defaultMat;

	void GetMaterials () {
		if (effectManager == null) {
			effectManager = FindObjectOfType<BodyEffectManager> ();
		}
		if (effectManager) {
			materials = effectManager.GetMaterials ();
		}

		if (materials == null) {
			materials = new List<Material> ();
		}

		if (materials.Count == 0) {
			if (defaultMat == null) {
				defaultMat = new Material (Shader.Find ("Unlit/Texture"));
			}
			materials.Add (defaultMat);
		}

	}

	[ImageEffectOpaque]
	private void OnRenderImage (RenderTexture src, RenderTexture dest) {
		GetMaterials ();
		renderSources.Clear ();
		renderTargets.Clear ();

		renderSources.Add (src);

		for (int i = 0; i < materials.Count - 1; i++) {
			var temp = RenderTexture.GetTemporary (src.width, src.height, 0, dest.graphicsFormat);
			renderTargets.Add (temp);
			renderSources.Add (temp);
		}

		renderTargets.Add (dest);

		for (int i = 0; i < materials.Count; i++) {
			// Bit does the following:
			// - sets _MainTex property on material to the source texture
			// - sets the render target to the destination texture
			// - draws a full-screen quad
			// This copies the src texture to the dest texture, with whatever modifications the shader makes
			Graphics.Blit (renderSources[i], renderTargets[i], materials[i]);
		}

		// Release all temporary render texture so they can be reused
		for (int i = 0; i < renderTargets.Count - 1; i++) {
			RenderTexture.ReleaseTemporary (renderTargets[i]);
		}

	}

}