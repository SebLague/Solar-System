using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Handles underwater and atmosphere effects of celestial bodies, which are achieved with post processing
public class BodyEffectManager : MonoBehaviour {

	public Shader oceanShader;
	public Shader atmosphereShader;
	public bool displayOceans = true;

	List<CelestialBodyGenerator> generators;
	List<float> sortDistances;

	OceanEffect[] oceanEffects;

	List<Material> postProcessingMaterials;
	bool active = true;

	void Init () {
		if (generators == null || generators.Count == 0 || Application.isEditor) {
			generators = new List<CelestialBodyGenerator> (FindObjectsOfType<CelestialBodyGenerator> ());
		}
		if (postProcessingMaterials == null) {
			postProcessingMaterials = new List<Material> ();
		}
		if (sortDistances == null) {
			sortDistances = new List<float> ();
		}
		sortDistances.Clear ();
		postProcessingMaterials.Clear ();

		if (displayOceans) {
			if (oceanEffects == null || oceanEffects.Length != generators.Count) {
				oceanEffects = new OceanEffect[generators.Count];
			}
		}
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.K)) {
			active = !active;
		}
	}

	public List<Material> GetMaterials () {

		if (!active) {
			return null;
		}
		Init ();

		if (generators.Count > 0) {
			Camera cam = Camera.current;
			Vector3 camPos = cam.transform.position;

			SortBodiesFarToNear (camPos);

			for (int i = 0; i < generators.Count; i++) {
				var generator = generators[i];
				// Oceans
				if (displayOceans) {
					if (generator.body.shading.hasOcean) {
						if (oceanEffects[i] == null) {
							oceanEffects[i] = new OceanEffect ();
						}

						oceanEffects[i].Update (generator, oceanShader);
						postProcessingMaterials.Add (oceanEffects[i].GetMaterial ());
					}
				}
			}
		}

		return postProcessingMaterials;
	}

	float CalculateMaxClippingDst (Camera cam) {
		float halfHeight = cam.nearClipPlane * Mathf.Tan (cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
		float halfWidth = halfHeight * cam.aspect;
		float dstToNearClipPlaneCorner = new Vector3 (halfWidth, halfHeight, cam.nearClipPlane).magnitude;
		return dstToNearClipPlaneCorner;
	}

	void SortBodiesFarToNear (Vector3 viewPos) {
		for (int i = 0; i < generators.Count; i++) {
			float dstToSurface = Mathf.Max (0, (generators[i].transform.position - viewPos).magnitude - generators[i].BodyScale);
			sortDistances.Add (dstToSurface);
		}

		for (int i = 0; i < generators.Count - 1; i++) {
			for (int j = i + 1; j > 0; j--) {
				if (sortDistances[j - 1] < sortDistances[j]) {
					float tempDst = sortDistances[j - 1];
					var tempGenerator = generators[j - 1];
					sortDistances[j - 1] = sortDistances[j];
					sortDistances[j] = tempDst;
					generators[j - 1] = generators[j];
					generators[j] = tempGenerator;
				}
			}
		}
	}
}