using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Moon/Moon Shape")]
public class MoonShape : CelestialBodyShape {

	public CraterSettings craterSettings;
	public SimpleNoiseSettings shapeNoise;
	public RidgeNoiseSettings ridgeNoise;
	public RidgeNoiseSettings ridgeNoise2;

	public Vector4 testParams;

	protected override void SetShapeData () {
		var prng = new PRNG (seed);

		SetCraterSettings (prng, seed, randomize);
		SetShapeNoiseSettings (prng, randomize);
		SetRidgeNoiseSettings (prng, randomize);

		heightMapCompute.SetVector ("testParams", testParams);
	}

	void SetCraterSettings (PRNG prng, int seed, bool randomizeValues) {
		if (randomizeValues) {
			var chance = new Chance (prng);
			if (chance.Percent (70)) { // Medium amount of mostly small to medium craters
				craterSettings.SetComputeValues (heightMapCompute, seed, prng.Range (100, 700), new Vector2 (0.01f, 0.1f), 0.57f);
			} else if (chance.Percent (15)) { // Many small craters
				craterSettings.SetComputeValues (heightMapCompute, seed, prng.Range (800, 1800), new Vector2 (0.01f, 0.08f), 0.74f);
			} else if (chance.Percent (15)) { // A few large craters
				craterSettings.SetComputeValues (heightMapCompute, seed, prng.Range (50, 150), new Vector2 (0.01f, 0.2f), 0.4f);
			}
		} else {
			craterSettings.SetComputeValues (heightMapCompute, seed);
		}
	}

	void SetShapeNoiseSettings (PRNG prng, bool randomizeValues) {
		const string suffix = "_shape";
		if (randomizeValues) {
			var chance = new Chance (prng);
			SimpleNoiseSettings randomizedShapeNoise = new SimpleNoiseSettings () {
				numLayers = 4, lacunarity = 2, persistence = 0.5f
			};

			if (chance.Percent (80)) { // Minor deformation
				randomizedShapeNoise.elevation = Mathf.Lerp (0.2f, 3, prng.ValueBiasLower (0.3f));
				randomizedShapeNoise.scale = prng.Range (1.5f, 2.5f);
			} else if (chance.Percent (20)) { // Major deformation
				randomizedShapeNoise.elevation = Mathf.Lerp (3, 8, prng.ValueBiasLower (0.4f));
				randomizedShapeNoise.scale = prng.Range (0.3f, 1);
			}

			// Assign settings
			randomizedShapeNoise.SetComputeValues (heightMapCompute, prng, suffix);

		} else {
			shapeNoise.SetComputeValues (heightMapCompute, prng, suffix);
		}
	}

	void SetRidgeNoiseSettings (PRNG prng, bool randomizeValues) {
		const string ridgeSuffix = "_ridge";
		const string detailSuffix = "_ridge2";
		if (randomizeValues) {
			// Randomize ridge mask
			var randomizedMaskNoise = new SimpleNoiseSettings () {
				numLayers = 4, lacunarity = 2, persistence = 0.6f, elevation = 1
			};
			randomizedMaskNoise.scale = prng.Range (0.5f, 2f);

			// Randomize ridge noise
			var chance = new Chance (prng);
			var randomizedRidgeNoise = new RidgeNoiseSettings () {
				numLayers = 4, power = 3, gain = 1, peakSmoothing = 2
			};

			randomizedRidgeNoise.elevation = Mathf.Lerp (1, 7, prng.ValueBiasLower (0.3f));
			randomizedRidgeNoise.scale = prng.Range (1f, 3f);
			randomizedRidgeNoise.lacunarity = prng.Range (1f, 5f);
			randomizedRidgeNoise.persistence = 0.42f;
			randomizedRidgeNoise.power = prng.Range (1.5f, 3.5f);

			randomizedRidgeNoise.SetComputeValues (heightMapCompute, prng, ridgeSuffix);
			randomizedMaskNoise.SetComputeValues (heightMapCompute, prng, detailSuffix);

		} else {
			ridgeNoise.SetComputeValues (heightMapCompute, prng, ridgeSuffix);
			ridgeNoise2.SetComputeValues (heightMapCompute, prng, detailSuffix);
		}
	}

	public override void ReleaseBuffers () {
		base.ReleaseBuffers ();
		craterSettings.ReleaseBuffers ();

	}

}