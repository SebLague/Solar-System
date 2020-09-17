using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CraterSettings {

	// Exposed settings
	public bool enabled = true;
	public int craterSeed;
	public int numCraters = 400;
	public Vector2 craterSizeMinMax = new Vector2 (0.01f, 0.1f);
	public float rimSteepness = 0.13f;
	public float rimWidth = 1.6f;
	public Vector2 smoothMinMax = new Vector2 (0.4f, 1.5f);
	[Range (0, 1)]
	public float sizeDistribution = 0.6f;

	[HideInInspector]
	public Crater[] cachedCraters;

	// Private
	ComputeBuffer craterBuffer;

	// Set values using exposed settings
	public void SetComputeValues (ComputeShader computeShader, int masterSeed) {
		SetComputeValues (computeShader, masterSeed, numCraters, craterSizeMinMax, sizeDistribution);
	}

	// Set values using custom numCraters
	public void SetComputeValues (ComputeShader computeShader, int masterSeed, int numCraters) {
		SetComputeValues (computeShader, masterSeed, numCraters, craterSizeMinMax, sizeDistribution);
	}

	// Set values using custom numCraters and sizeMinMax
	public void SetComputeValues (ComputeShader computeShader, int masterSeed, int numCraters, Vector2 craterSizeMinMax, float sizeDistribution) {
		if (!enabled) {
			numCraters = 1;
			craterSizeMinMax = Vector2.zero;
		}

		Random.InitState (craterSeed + masterSeed);
		Crater[] craters = new Crater[numCraters];
		PRNG prng = new PRNG (masterSeed);

		// Create craters
		for (int i = 0; i < numCraters; i++) {
			float t = prng.ValueBiasLower (sizeDistribution);

			float size = Mathf.Lerp (craterSizeMinMax.x, craterSizeMinMax.y, t);
			float floorHeight = Mathf.Lerp (-1.2f, -0.2f, t + prng.ValueBiasLower (0.3f));
			float smooth = Mathf.Lerp (smoothMinMax.x, smoothMinMax.y, 1 - t);
			craters[i] = new Crater () { centre = Random.onUnitSphere, size = size, floorHeight = floorHeight, smoothness = smooth };
		}
		cachedCraters = craters;

		// Set shape data
		ComputeHelper.CreateAndSetBuffer<Crater> (ref craterBuffer, craters, computeShader, "craters");
		computeShader.SetInt ("numCraters", numCraters);

		computeShader.SetFloat (nameof (rimSteepness), rimSteepness);
		computeShader.SetFloat (nameof (rimWidth), rimWidth);
		//computeShader.SetFloat (nameof (smoothFactor), smoothFactor);
	}

	public void ReleaseBuffers () {
		ComputeHelper.Release (craterBuffer);
	}

	[System.Serializable]
	public struct Crater {
		public Vector3 centre;
		public float size;
		public float floorHeight;
		public float smoothness;
	}

}