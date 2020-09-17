using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemSpawner : MonoBehaviour {

	public CelestialBodyGenerator.ResolutionSettings resolutionSettings;

	void Awake () {
		Spawn (0);
	}

	public void Spawn (int seed) {

		var sw = System.Diagnostics.Stopwatch.StartNew ();

		PRNG prng = new PRNG (seed);
		CelestialBody[] bodies = FindObjectsOfType<CelestialBody> ();

		foreach (var body in bodies) {
			if (body.bodyType == CelestialBody.BodyType.Sun) {
				continue;
			}

			BodyPlaceholder placeholder = body.gameObject.GetComponentInChildren<BodyPlaceholder> ();
			var template = placeholder.bodySettings;

			Destroy (placeholder.gameObject);

			GameObject holder = new GameObject ("Body Generator");
			var generator = holder.AddComponent<CelestialBodyGenerator> ();
			generator.transform.parent = body.transform;
			generator.gameObject.layer = body.gameObject.layer;
			generator.transform.localRotation = Quaternion.identity;
			generator.transform.localPosition = Vector3.zero;
			generator.transform.localScale = Vector3.one * body.radius;
			generator.resolutionSettings = resolutionSettings;

			generator.body = template;

		}

		Debug.Log ("Generation time: " + sw.ElapsedMilliseconds + " ms.");
	}

}