using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Responsible for rendering oceans and atmospheres as post processing effect
*/

[CreateAssetMenu (menuName = "PostProcessing/PlanetEffects")]
public class PlanetEffects : PostProcessingEffect {

	public Shader oceanShader;
	public Shader atmosphereShader;
	public bool displayOceans = true;
	public bool displayAtmospheres = true;

	List<EffectHolder> effectHolders;
	List<float> sortDistances;

	List<Material> postProcessingMaterials;
	bool active = true;

	public override void Render (RenderTexture source, RenderTexture destination) {
		List<Material> materials = GetMaterials ();
		CustomPostProcessing.RenderMaterials (source, destination, materials);
	}

	void Init () {
		if (effectHolders == null || effectHolders.Count == 0 || !Application.isPlaying) {
			var generators = FindObjectsOfType<CelestialBodyGenerator> ();
			effectHolders = new List<EffectHolder> (generators.Length);
			for (int i = 0; i < generators.Length; i++) {
				effectHolders.Add (new EffectHolder (generators[i]));
			}
		}
		if (postProcessingMaterials == null) {
			postProcessingMaterials = new List<Material> ();
		}
		if (sortDistances == null) {
			sortDistances = new List<float> ();
		}
		sortDistances.Clear ();
		postProcessingMaterials.Clear ();
	}

	public List<Material> GetMaterials () {

		if (!active) {
			return null;
		}
		Init ();

		if (effectHolders.Count > 0) {
			Camera cam = Camera.current;
			Vector3 camPos = cam.transform.position;

			SortFarToNear (camPos);

			for (int i = 0; i < effectHolders.Count; i++) {
				EffectHolder effectHolder = effectHolders[i];
				Material underwaterMaterial = null;
				// Oceans
				if (displayOceans) {
					if (effectHolder.oceanEffect != null) {

						effectHolder.oceanEffect.UpdateSettings (effectHolder.generator, oceanShader);

						float camDstFromCentre = (camPos - effectHolder.generator.transform.position).magnitude;
						if (camDstFromCentre < effectHolder.generator.GetOceanRadius ()) {
							underwaterMaterial = effectHolder.oceanEffect.GetMaterial ();
						} else {
							postProcessingMaterials.Add (effectHolder.oceanEffect.GetMaterial ());
						}
					}
				}
				// Atmospheres
				if (displayAtmospheres) {
					if (effectHolder.atmosphereEffect != null) {
						effectHolder.atmosphereEffect.UpdateSettings (effectHolder.generator);
						postProcessingMaterials.Add (effectHolder.atmosphereEffect.GetMaterial ());
					}
				}

				if (underwaterMaterial != null) {
					postProcessingMaterials.Add (underwaterMaterial);
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

	public class EffectHolder {
		public CelestialBodyGenerator generator;
		public OceanEffect oceanEffect;
		public AtmosphereEffect atmosphereEffect;

		public EffectHolder (CelestialBodyGenerator generator) {
			this.generator = generator;
			if (generator.body.shading.hasOcean && generator.body.shading.oceanSettings) {
				oceanEffect = new OceanEffect ();
			}
			if (generator.body.shading.hasAtmosphere && generator.body.shading.atmosphereSettings) {
				atmosphereEffect = new AtmosphereEffect ();
			}
		}

		public float DstFromSurface (Vector3 viewPos) {
			return Mathf.Max (0, (generator.transform.position - viewPos).magnitude - generator.BodyScale);
		}
	}

	void SortFarToNear (Vector3 viewPos) {
		for (int i = 0; i < effectHolders.Count; i++) {
			float dstToSurface = effectHolders[i].DstFromSurface (viewPos);
			sortDistances.Add (dstToSurface);
		}

		for (int i = 0; i < effectHolders.Count - 1; i++) {
			for (int j = i + 1; j > 0; j--) {
				if (sortDistances[j - 1] < sortDistances[j]) {
					float tempDst = sortDistances[j - 1];
					var temp = effectHolders[j - 1];
					sortDistances[j - 1] = sortDistances[j];
					sortDistances[j] = tempDst;
					effectHolders[j - 1] = effectHolders[j];
					effectHolders[j] = temp;
				}
			}
		}
	}
}