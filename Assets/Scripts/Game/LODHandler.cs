using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LODHandler : MonoBehaviour {
	[Header ("LOD screen heights")]
	// LOD level is determined by body's screen height (1 = taking up entire screen, 0 = teeny weeny speck) 
	public float lod1Threshold = .5f;
	public float lod2Threshold = .2f;

	[Header ("Debug")]
	public bool debug;
	public CelestialBody debugBody;

	Camera cam;
	Transform camT;
	CelestialBody[] bodies;
	CelestialBodyGenerator[] generators;

	void Start () {
		if (Application.isPlaying) {
			bodies = FindObjectsOfType<CelestialBody> ();
			generators = new CelestialBodyGenerator[bodies.Length];
			for (int i = 0; i < generators.Length; i++) {
				generators[i] = bodies[i].GetComponentInChildren<CelestialBodyGenerator> ();
			}
		}
	}

	void Update () {
		DebugLODInfo ();

		if (Application.isPlaying) {
			HandleLODs ();
		}

	}

	void HandleLODs () {
		for (int i = 0; i < bodies.Length; i++) {
			if (generators[i] != null) {
				float screenHeight = CalculateScreenHeight (bodies[i]);
				int lodIndex = CalculateLODIndex (screenHeight);
				generators[i].SetLOD (lodIndex);
			}

		}
	}

	int CalculateLODIndex (float screenHeight) {
		if (screenHeight > lod1Threshold) {
			return 0;
		} else if (screenHeight > lod2Threshold) {
			return 1;
		}
		return 2;
	}

	void DebugLODInfo () {
		if (debugBody && debug) {
			float h = CalculateScreenHeight (debugBody);
			int index = CalculateLODIndex (h);
			Debug.Log ($"Screen height of {debugBody.name}: {h} (lod = {index})");
		}
	}

	float CalculateScreenHeight (CelestialBody body) {
		if (cam == null) {
			cam = Camera.main;
			camT = cam.transform;
		}
		Quaternion originalRot = camT.rotation;
		Vector3 bodyCentre = body.transform.position;
		camT.LookAt (bodyCentre);

		Vector3 viewA = cam.WorldToViewportPoint (bodyCentre - camT.up * body.radius);
		Vector3 viewB = cam.WorldToViewportPoint (bodyCentre + camT.up * body.radius);
		float screenHeight = Mathf.Abs (viewA.y - viewB.y);
		camT.rotation = originalRot;

		return screenHeight;
	}
}