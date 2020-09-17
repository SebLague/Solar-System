using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColourTest : MonoBehaviour {

	public MeshRenderer[] renderers;

	public Vector2 saturationMinMax;
	public Vector2 valueMinMax;
	public int seed;
	Material[] materials;

	void Update () {
		Process ();
	}

	public void Random () {
		seed = UnityEngine.Random.Range (-1000, 1000);
		Process ();
	}

	void Process () {
		if (materials == null || materials.Length != renderers.Length) {
			materials = new Material[renderers.Length];
		}

		var random = new PRNG (seed);
		for (int i = 0; i < renderers.Length; i++) {
			if (materials[i] == null) {
				materials[i] = new Material (Shader.Find ("Unlit/Color"));
			}
			var col = ColourHelper.Random (random, saturationMinMax.x, saturationMinMax.y, valueMinMax.x, valueMinMax.y);
			materials[i].color = col;
			renderers[i].sharedMaterial = materials[i];
		}
	}
}