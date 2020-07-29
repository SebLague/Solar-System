using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OceanEffect : PostProcessingEffect {

	Light light;

	public void Update (CelestialBodyGenerator generator, Shader shader) {
		if (material == null || material.shader != shader) {
			material = new Material (shader);
		}

		if (light == null) {
			light = GameObject.FindObjectOfType<SunShadowCaster> ().GetComponent<Light> ();
		}

		Vector3 centre = generator.transform.position;
		float radius = generator.GetOceanRadius ();
		material.SetVector ("oceanCentre", centre);
		material.SetFloat ("oceanRadius", radius);

		material.SetFloat ("planetScale", generator.BodyScale);
		material.SetVector ("dirToSun", -light.transform.forward);
		generator.body.shading.SetOceanProperties (material);
	}

	public override Material GetMaterial () {
		return material;
	}

}