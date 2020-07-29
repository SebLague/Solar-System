using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Earth-Like/Earth Shading")]
public class EarthShading : CelestialBodyShading {

	public EarthColours customizedCols;
	public EarthColours randomizedCols;

	[Header ("Shading Data")]
	public SimpleNoiseSettings detailWarpNoise;
	public SimpleNoiseSettings detailNoise;
	public SimpleNoiseSettings largeNoise;
	public SimpleNoiseSettings smallNoise;

	public override void SetTerrainProperties (Material material, Vector2 heightMinMax, float bodyScale) {

		material.SetVector ("heightMinMax", heightMinMax);
		material.SetFloat ("oceanLevel", oceanLevel);
		material.SetFloat ("bodyScale", bodyScale);

		if (randomize) {
			SetRandomColours (material);
			ApplyColours (material, randomizedCols);
		} else {
			ApplyColours (material, customizedCols);
		}
	}

	void ApplyColours (Material material, EarthColours colours) {
		material.SetColor ("_ShoreLow", colours.shoreColLow);
		material.SetColor ("_ShoreHigh", colours.shoreColHigh);

		material.SetColor ("_FlatLowA", colours.flatColLowA);
		material.SetColor ("_FlatHighA", colours.flatColHighA);

		material.SetColor ("_FlatLowB", colours.flatColLowB);
		material.SetColor ("_FlatHighB", colours.flatColHighB);

		material.SetColor ("_SteepLow", colours.steepLow);
		material.SetColor ("_SteepHigh", colours.steepHigh);
	}

	void SetRandomColours (Material material) {
		PRNG random = new PRNG (seed);
		//randomizedCols.shoreCol = ColourHelper.Random (random, 0.3f, 0.7f, 0.4f, 0.8f);
		randomizedCols.flatColLowA = ColourHelper.Random (random, 0.45f, 0.6f, 0.7f, 0.8f);
		randomizedCols.flatColHighA = ColourHelper.TweakHSV (
			randomizedCols.flatColLowA,
			random.SignedValue () * 0.2f,
			random.SignedValue () * 0.15f,
			random.Range (-0.25f, -0.2f)
		);

		randomizedCols.flatColLowB = ColourHelper.Random (random, 0.45f, 0.6f, 0.7f, 0.8f);
		randomizedCols.flatColHighB = ColourHelper.TweakHSV (
			randomizedCols.flatColLowB,
			random.SignedValue () * 0.2f,
			random.SignedValue () * 0.15f,
			random.Range (-0.25f, -0.2f)
		);

		randomizedCols.shoreColLow = ColourHelper.Random (random, 0.2f, 0.3f, 0.9f, 1);
		randomizedCols.shoreColHigh = ColourHelper.TweakHSV (
			randomizedCols.shoreColLow,
			random.SignedValue () * 0.2f,
			random.SignedValue () * 0.2f,
			random.Range (-0.3f, -0.2f)
		);

		randomizedCols.steepLow = ColourHelper.Random (random, 0.3f, 0.7f, 0.4f, 0.6f);
		randomizedCols.steepHigh = ColourHelper.TweakHSV (
			randomizedCols.steepLow,
			random.SignedValue () * 0.2f,
			random.SignedValue () * 0.2f,
			random.Range (-0.35f, -0.2f)
		);
	}

	protected override void SetShadingDataComputeProperties () {
		PRNG random = new PRNG (seed);
		detailNoise.SetComputeValues (shadingDataCompute, random, "_detail");
		detailWarpNoise.SetComputeValues (shadingDataCompute, random, "_detailWarp");
		largeNoise.SetComputeValues (shadingDataCompute, random, "_large");
		smallNoise.SetComputeValues (shadingDataCompute, random, "_small");
	}

	[System.Serializable]
	public struct EarthColours {
		public Color shoreColLow;
		public Color shoreColHigh;
		public Color flatColLowA;
		public Color flatColHighA;
		public Color flatColLowB;
		public Color flatColHighB;

		public Color steepLow;
		public Color steepHigh;
	}
}