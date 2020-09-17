using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

[CreateAssetMenu (menuName = "Celestial Body/Atmosphere")]
public class AtmosphereSettings : ScriptableObject {

	public bool enabled = true;
	public Shader atmosphereShader;
	public ComputeShader opticalDepthCompute;
	public int textureSize = 256;

	public int inScatteringPoints = 10;
	public int opticalDepthPoints = 10;
	public float densityFalloff = 0.25f;

	public Vector3 wavelengths = new Vector3 (700, 530, 460);

	public Vector4 testParams = new Vector4 (7, 1.26f, 0.1f, 3);
	public float scatteringStrength = 20;
	public float intensity = 1;

	public float ditherStrength = 0.8f;
	public float ditherScale = 4;
	public Texture2D blueNoise;

	[Range (0, 1)]
	public float atmosphereScale = 0.5f;

	[Header ("Test")]
	public float timeOfDay;
	public float sunDst = 1;

	RenderTexture opticalDepthTexture;
	bool settingsUpToDate;

	public void SetProperties (Material material, float bodyRadius) {
		/*
		if (Application.isPlaying) {
			if (Time.time > 1) {
				timeOfDay += Time.deltaTime * 0.1f;
					var sun = GameObject.Find ("Test Sun");
			sun.transform.position = new Vector3 (Mathf.Cos (timeOfDay), Mathf.Sin (timeOfDay), 0) * sunDst;
			sun.transform.LookAt (Vector3.zero);
			}
		}
		*/
		if (!settingsUpToDate || !Application.isPlaying) {
			var sun = GameObject.Find ("Test Sun");
			if (sun) {
				sun.transform.position = new Vector3 (Mathf.Cos (timeOfDay), Mathf.Sin (timeOfDay), 0) * sunDst;
				sun.transform.LookAt (Vector3.zero);
			}

			float atmosphereRadius = (1 + atmosphereScale) * bodyRadius;

			material.SetVector ("params", testParams);
			material.SetInt ("numInScatteringPoints", inScatteringPoints);
			material.SetInt ("numOpticalDepthPoints", opticalDepthPoints);
			material.SetFloat ("atmosphereRadius", atmosphereRadius);
			material.SetFloat ("planetRadius", bodyRadius);
			material.SetFloat ("densityFalloff", densityFalloff);

			// Strength of (rayleigh) scattering is inversely proportional to wavelength^4
			float scatterX = Pow (400 / wavelengths.x, 4);
			float scatterY = Pow (400 / wavelengths.y, 4);
			float scatterZ = Pow (400 / wavelengths.z, 4);
			material.SetVector ("scatteringCoefficients", new Vector3 (scatterX, scatterY, scatterZ) * scatteringStrength);
			material.SetFloat ("intensity", intensity);
			material.SetFloat ("ditherStrength", ditherStrength);
			material.SetFloat ("ditherScale", ditherScale);
			material.SetTexture ("_BlueNoise", blueNoise);

			PrecomputeOutScattering ();
			material.SetTexture ("_BakedOpticalDepth", opticalDepthTexture);

			settingsUpToDate = true;
		}
	}

	void PrecomputeOutScattering () {
		if (!settingsUpToDate || opticalDepthTexture == null || !opticalDepthTexture.IsCreated ()) {
			ComputeHelper.CreateRenderTexture (ref opticalDepthTexture, textureSize, FilterMode.Bilinear);
			opticalDepthCompute.SetTexture (0, "Result", opticalDepthTexture);
			opticalDepthCompute.SetInt ("textureSize", textureSize);
			opticalDepthCompute.SetInt ("numOutScatteringSteps", opticalDepthPoints);
			opticalDepthCompute.SetFloat ("atmosphereRadius", (1 + atmosphereScale));
			opticalDepthCompute.SetFloat ("densityFalloff", densityFalloff);
			opticalDepthCompute.SetVector ("params", testParams);
			ComputeHelper.Run (opticalDepthCompute, textureSize, textureSize);
		}

	}

	void OnValidate () {
		settingsUpToDate = false;
	}
}