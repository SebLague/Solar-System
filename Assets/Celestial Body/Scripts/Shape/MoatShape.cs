using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Moat/Moat Shape")]
public class MoatShape : CelestialBodyShape {

	[Header ("Continent settings")]
	public float shoreSteepness = 4;
	public float continentLevel = -0.05f;
	public float continentFlatness = 0.27f;

	[Header ("Mountain settings")]
	public float mountainSmoothing = 1f; // Reduces excessive jaggedness of mountains in some regions
	public float mountainBlend = 1; // Determines how smoothly the base of mountains blends into the terrain
	public Vector2 maskMinMax = new Vector2 (0.35f, 0.5f);

	[Header ("Noise settings")]
	public SimpleNoiseSettings continentNoise;
	public CraterSettings craterSettings;

	public Vector4 testParams;

	protected override void SetShapeData () {
		var prng = new PRNG (seed);
		continentNoise.SetComputeValues (heightMapCompute, prng, "_continents");
		craterSettings.SetComputeValues (heightMapCompute, seed);

		heightMapCompute.SetFloat ("shoreSteepness", shoreSteepness);
		heightMapCompute.SetFloat ("continentFlatness", continentFlatness);
		heightMapCompute.SetFloat ("continentLevel", continentLevel);
		heightMapCompute.SetFloat ("mountainSmoothing", mountainSmoothing);
		heightMapCompute.SetFloat ("mountainBlend", mountainBlend);
		heightMapCompute.SetVector ("maskMinMax", maskMinMax);
		heightMapCompute.SetVector ("params", testParams);
	}

	public override void ReleaseBuffers () {
		base.ReleaseBuffers ();
		craterSettings.ReleaseBuffers ();

	}

}