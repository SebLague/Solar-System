using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Shapes/Shattered")]
public class ShatteredShape : CelestialBodyShape {

	[Header ("Noise settings")]
	public float plateauHeight;
	public float plateauSmoothing;
	public SimpleNoiseSettings continentNoise;
	public SimpleNoiseSettings warpNoise;
	public RidgeNoiseSettings ridgeNoise;
	public RidgeNoiseSettings ridgeNoise2;
	public CraterSettings craterSettings;
	public Vector4 testParams;

	protected override void SetShapeData () {
		var prng = new PRNG (seed);
		continentNoise.SetComputeValues (heightMapCompute, prng, "_continents");
		ridgeNoise.SetComputeValues (heightMapCompute, prng, "_mountains");
		ridgeNoise2.SetComputeValues (heightMapCompute, prng, "_mountains2");
		warpNoise.SetComputeValues (heightMapCompute, prng, "_warp");
		craterSettings.SetComputeValues (heightMapCompute, seed);

		heightMapCompute.SetFloat ("plateauHeight", plateauHeight);
		heightMapCompute.SetFloat ("plateauSmoothing", plateauSmoothing);
		heightMapCompute.SetVector ("params", testParams);
	}

	public override void ReleaseBuffers () {
		base.ReleaseBuffers ();
		craterSettings.ReleaseBuffers ();

	}
}