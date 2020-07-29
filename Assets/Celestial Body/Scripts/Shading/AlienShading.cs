using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Alein/Alien Shading")]
public class AlienShading : CelestialBodyShading {

	public AlienColours customizedCols;
	public AlienColours randomizedCols;

	[Header ("Shading Data")]
	public SimpleNoiseSettings detailWarpNoise;
	public SimpleNoiseSettings detailNoise;
	public SimpleNoiseSettings largeNoise;
	public SimpleNoiseSettings smallNoise;

	public SimpleNoiseSettings noise2;
	public SimpleNoiseSettings warp2;

	public override void SetTerrainProperties (Material material, Vector2 heightMinMax, float bodyScale) {

		material.SetVector ("heightMinMax", heightMinMax);
		material.SetFloat ("oceanLevel", oceanLevel);
		material.SetFloat("bodyScale", bodyScale);

		if (randomize) {
			SetRandomColours (material);
			ApplyColours (material, randomizedCols);
		} else {
			ApplyColours (material, customizedCols);
		}

	}

	void ApplyColours (Material material, AlienColours colours) {
		material.SetColor ("_ShoreCol", colours.shoreCol);
		material.SetColor ("_FlatColA", colours.flatColA1);
		material.SetColor ("_FlatColA2", colours.flatColA2);
		material.SetColor ("_FlatColB", colours.flatColB1);
		material.SetColor ("_FlatColB2", colours.flatColB2);
		material.SetColor ("_SteepColA", colours.steepColA);
		material.SetColor ("_SteepColB", colours.steepColB);
	}

	void SetRandomColours (Material material) {
		PRNG random = new PRNG (seed);
		randomizedCols.shoreCol = ColourHelper.Random (random, 0.3f, 0.7f, 0.4f, 0.8f);
		randomizedCols.flatColA1 = ColourHelper.RandomSimilar (random, randomizedCols.shoreCol);
		randomizedCols.flatColB1 = ColourHelper.RandomSimilar (random, randomizedCols.flatColA1);

		randomizedCols.flatColA2 = ColourHelper.RandomSimilar (random, randomizedCols.flatColA1);
		randomizedCols.flatColB2 = ColourHelper.RandomSimilar (random, randomizedCols.flatColA2);

		randomizedCols.steepColA = ColourHelper.Random (random, 0.3f, 0.7f, 0.2f, 0.85f);
		randomizedCols.steepColB = ColourHelper.RandomSimilar (random, randomizedCols.steepColA);
	}

	protected override void SetShadingDataComputeProperties () {
		PRNG random = new PRNG (seed);
		detailNoise.SetComputeValues (shadingDataCompute, random, "_detail");
		detailWarpNoise.SetComputeValues (shadingDataCompute, random, "_detailWarp");
		largeNoise.SetComputeValues (shadingDataCompute, random, "_large");
		smallNoise.SetComputeValues (shadingDataCompute, random, "_small");

		noise2.SetComputeValues (shadingDataCompute, random, "_noise2");
		warp2.SetComputeValues (shadingDataCompute, random, "_warp2");
	}

	[System.Serializable]
	public struct AlienColours {
		public Color shoreCol;
		public Color flatColA1;
		public Color flatColA2;
		public Color flatColB1;
		public Color flatColB2;
		public Color steepColA;
		public Color steepColB;
	}
}