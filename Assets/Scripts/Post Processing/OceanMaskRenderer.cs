using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OceanMaskRenderer : MonoBehaviour {

	public Shader oceanMaskShader;

	[HideInInspector]
	public RenderTexture oceanMaskTexture;
	CelestialBodyGenerator[] oceanBodies;
	RenderTexture prev;

	void Update () {
		Init ();
	}

	void Init () {
		if (!Application.isPlaying || oceanBodies == null) {
			var allBodies = FindObjectsOfType<CelestialBodyGenerator> ();
			var oceanBodiesList = new List<CelestialBodyGenerator> ();
			for (int i = 0; i < allBodies.Length; i++) {
				if (allBodies[i].body.shading.hasOcean && allBodies[i].body.shading.oceanSettings != null) {
					oceanBodiesList.Add (allBodies[i]);
				}
			}
			oceanBodies = oceanBodiesList.ToArray ();
			FindObjectOfType<CustomPostProcessing> ().onPostProcessingBegin -= RenderOceanMask;
			FindObjectOfType<CustomPostProcessing> ().onPostProcessingBegin += RenderOceanMask;
		}

	}

	void RenderOceanMask (RenderTexture screenTex) {

		Init ();

		if (prev != null) {
			prev.Release ();
			prev = null;
		}

		if (oceanMaskTexture == null || oceanMaskTexture.width != screenTex.width || oceanMaskTexture.height != screenTex.height) {
			if (oceanMaskTexture != null) {
				prev = oceanMaskTexture;
			}
			oceanMaskTexture = new RenderTexture (screenTex);
		}

		oceanMaskTexture.Create ();
		if (oceanBodies != null && oceanBodies.Length > 0) {
			var mat = new Material (oceanMaskShader);

			Vector4[] oceanSpheres = new Vector4[oceanBodies.Length];
			for (int i = 0; i < oceanBodies.Length; i++) {
				Vector3 pos = oceanBodies[i].transform.position;
				float oceanRadius = oceanBodies[i].GetOceanRadius ();
				oceanSpheres[i] = new Vector4 (pos.x, pos.y, pos.z, oceanRadius);
			}
			mat.SetInt ("numSpheres", oceanSpheres.Length);
			mat.SetVectorArray ("spheres", oceanSpheres);
			//ComputeHelper.Run (oceanMaskCompute, width, height);

			Graphics.Blit (screenTex, oceanMaskTexture, mat);
		}

	}

	void OnDestroy () {
		if (oceanMaskTexture != null) {
			oceanMaskTexture.Release ();
		}
	}
}