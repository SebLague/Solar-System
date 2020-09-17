using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Shapes/Alien")]
public class AlienShape : CelestialBodyShape {

	[Header ("Continent settings")]
	public float oceanDepthMultiplier = 5;
	public float oceanFloorDepth = 1.5f;
	public float oceanFloorSmoothing = 0.5f;

	public float mountainBlend = 1.2f; // Determines how smoothly the base of mountains blends into the terrain

	[Header ("Noise settings")]
	public SimpleNoiseSettings continentNoise;
	public SimpleNoiseSettings maskNoise;
	public SimpleNoiseSettings warpNoise;
	public RidgeNoiseSettings ridgeNoise;
	public CraterSettings craterSettings;
	public Vector4 testParams;

	protected override void SetShapeData () {
		var prng = new PRNG (seed);
		continentNoise.SetComputeValues (heightMapCompute, prng, "_continents");
		ridgeNoise.SetComputeValues (heightMapCompute, prng, "_mountains");
		maskNoise.SetComputeValues (heightMapCompute, prng, "_mask");
		warpNoise.SetComputeValues (heightMapCompute, prng, "_warp");
		craterSettings.SetComputeValues (heightMapCompute, seed);

		heightMapCompute.SetFloat ("oceanDepthMultiplier", oceanDepthMultiplier);
		heightMapCompute.SetFloat ("oceanFloorDepth", oceanFloorDepth);
		heightMapCompute.SetFloat ("oceanFloorSmoothing", oceanFloorSmoothing);
		heightMapCompute.SetFloat ("mountainBlend", mountainBlend);
		heightMapCompute.SetVector ("params", testParams);
	}

	public override void ReleaseBuffers () {
		base.ReleaseBuffers ();
		craterSettings.ReleaseBuffers ();

	}
}