using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Moon/Moon Shading")]
public class MoonShading : CelestialBodyShading {

	public Color primaryColA = Color.white;
	public Color secondaryColA = Color.black;
	public Color primaryColB = Color.white;
	public Color secondaryColB = Color.black;
	public Color steepCol = Color.black;

	[Header ("Shading Data")]
	public int numBiomePoints = 20;
	public Vector2 radiusMinMax = new Vector2 (0.02f, 0.1f);

	public SimpleNoiseSettings biomeWarpNoise;
	public SimpleNoiseSettings detailNoise;
	public SimpleNoiseSettings detailWarpNoise;

	[Header ("Rays")]
	[Range (0, 1)]
	public float candidatePoolSize = 0.2f;
	public int desiredNumCraterRays = 2;
	public int ejectaRaySeed;
	public float ejectaRaysScale = 10;

	[Header ("Normal maps")]
	public Texture2D[] normalMapsFlat;
	public Texture2D[] normalMapsSteep;

	ComputeBuffer craterBuffer;
	ComputeBuffer pointBuffer;
	MoonShape moonShape;

	public override void Initialize (CelestialBodyShape shape) {
		base.Initialize (shape);
		moonShape = shape as MoonShape;
	}

	public override void SetTerrainProperties (Material material, Vector2 heightMinMax, float bodyScale) {
		material.SetVector ("heightMinMax", heightMinMax);
		material.SetFloat ("bodyScale", bodyScale);

		var prng = new PRNG (seed);

		if (randomize) {

			SetColours (prng, material);

			material.SetFloat ("_SmoothnessA", Mathf.Lerp (0f, 0.6f, prng.SmallestRandom01 (4)));
			material.SetFloat ("_SmoothnessB", Mathf.Lerp (0f, 0.6f, prng.SmallestRandom01 (4)));
			material.SetFloat ("_Metallic", Mathf.Lerp (0f, 0.5f, prng.SmallestRandom01 (3)));

			var randomNormalMapFlat = prng.RandomElement (normalMapsFlat);
			var randomNormalMapSteep = randomNormalMapFlat;
			int loopSafety = 0;
			while (randomNormalMapSteep == randomNormalMapFlat && loopSafety < 20) {
				randomNormalMapSteep = prng.RandomElement (normalMapsSteep);
				loopSafety++;
			}
			material.SetTexture ("_NormalMapFlat", randomNormalMapFlat);
			material.SetTexture ("_NormalMapSteep", randomNormalMapSteep);

			SetBiomeSettings (prng, material);

		} else {
			material.SetColor ("_PrimaryColA", primaryColA);
			material.SetColor ("_SecondaryColA", secondaryColA);
			material.SetColor ("_PrimaryColB", primaryColB);
			material.SetColor ("_SecondaryColB", secondaryColB);
			material.SetColor ("_SteepCol", steepCol);
		}

		//
		if (cachedShadingData != null) {
			float biomeNoiseSum = 0;
			for (int i = 0; i < cachedShadingData.Length; i++) {
				biomeNoiseSum += cachedShadingData[i].w;
			}
			material.SetFloat ("_AvgBiomeNoiseDst", biomeNoiseSum / cachedShadingData.Length);
		} else {
			Debug.LogError ("Cached shading noise null");
			material.SetFloat ("_AvgBiomeNoiseDst", 5);
		}

	}

	void SetBiomeSettings (PRNG prng, Material material) {
		var biomeValues = new Vector4 (
			prng.SignedValueBiasExtremes (0.3f),
			prng.SignedValueBiasExtremes (0.3f) * 0.4f,
			prng.SignedValueBiasExtremes (0.3f) * 0.3f,
			prng.SignedValueBiasCentre (0.3f) * .7f
		);
		material.SetVector ("_RandomBiomeValues", biomeValues);
		float warpStrength = prng.SignedValueBiasCentre (.65f) * 30;
		material.SetFloat ("_BiomeBlendStrength", prng.Range (2f, 12) + Mathf.Abs (warpStrength) / 2);
		material.SetFloat ("_BiomeWarpStrength", warpStrength);
	}

	void SetColours (PRNG rand, Material material) {

		var colChance = new Chance (rand);
		var primaryA_HSV = Vector3.zero;
		var secondaryA_HSV = Vector3.zero;
		var primaryB_HSV = Vector3.zero;
		var secondaryB_HSV = Vector3.zero;

		// One light grey, one dark grey
		if (colChance.Percent (25)) {
			primaryA_HSV = new Vector3 (0, 0, Mathf.Lerp (0.55f, 1, rand.ValueBiasUpper (.4f)));
			secondaryA_HSV = primaryA_HSV + new Vector3 (0, 0, rand.SignedValueBiasCentre (0.4f));
			primaryB_HSV = new Vector3 (0, 0, Mathf.Lerp (0f, 0.45f, rand.ValueBiasLower (.4f)));
			secondaryB_HSV = primaryB_HSV + new Vector3 (0, 0, rand.SignedValueBiasCentre (0.4f));
		}
		// One colour, one grey
		else if (colChance.Percent (25)) {
			// Pick grey, tending towards either very dark or very light
			float greyValue = rand.ValueBiasExtremes (0.8f);
			primaryA_HSV = new Vector3 (0, 0, greyValue);
			secondaryA_HSV = new Vector3 (0, 0, greyValue + rand.SignedValueBiasCentre (0.5f) * .3f);
			// If grey is dark, use bright colour, otherwise dark colour
			float colourValue = (greyValue < 0.5f) ? rand.ValueBiasUpper (0.7f) : rand.ValueBiasLower (0.7f);
			primaryB_HSV = new Vector3 (rand.Value (), rand.Range (0.2f, 0.9f), Mathf.Lerp (0.1f, 0.9f, colourValue));
			secondaryB_HSV = primaryB_HSV + rand.JiggleVector3 (0.1f, 0.2f, 0.4f);
		}

		// Two similar colours
		else if (colChance.Percent (25)) {
			primaryA_HSV = new Vector3 (rand.Range (0, 1), rand.Range (0.1f, 0.8f), rand.Range (0.2f, 0.8f));
			secondaryA_HSV = primaryA_HSV + rand.JiggleVector3 (0.1f, 0.2f, 0.3f);
			primaryB_HSV = new Vector3 (primaryA_HSV.x + rand.Range (0.05f, 0.1f), rand.Range (0.1f, 0.8f), rand.Range (0.2f, 0.8f));
			secondaryB_HSV = primaryB_HSV + rand.JiggleVector3 (0.1f, 0.2f, 0.3f);
		}

		// Two distinct colours
		else if (colChance.Percent (25)) {
			primaryA_HSV = new Vector3 (rand.Value (), rand.Range (0.2f, 0.9f), rand.Range (0.1f, 0.9f));
			secondaryA_HSV = primaryA_HSV + rand.JiggleVector3 (0.1f, 0.2f, 0.3f);
			primaryB_HSV = new Vector3 ((primaryA_HSV.x + rand.Range (0.2f, 0.8f)) % 1, rand.Range (0.2f, 0.9f), rand.Range (0.1f, 0.9f));
			secondaryB_HSV = primaryB_HSV + rand.JiggleVector3 (0.1f, 0.2f, 0.3f);

		}

		material.SetColor ("_PrimaryColA", HSVToRGB (primaryA_HSV));
		material.SetColor ("_SecondaryColA", HSVToRGB (secondaryA_HSV));
		material.SetColor ("_PrimaryColB", HSVToRGB (primaryB_HSV));
		material.SetColor ("_SecondaryColB", HSVToRGB (secondaryB_HSV));
	}

	Color GreyscaleColor (float value) {
		return new Color (value, value, value, 1);
	}

	Color HSVToRGB (Vector3 col) {
		return Color.HSVToRGB (Mathf.Clamp01 (col.x), Mathf.Clamp01 (col.y), Mathf.Clamp01 (col.z));
	}

	protected override void SetShadingDataComputeProperties () {
		SetCraters (moonShape);
		SetRandomPoints ();
		SetShadingNoise ();
	}

	void SetShadingNoise () {
		const string biomeWarpNoiseSuffix = "_biomeWarp";
		const string detailWarpNoiseSuffix = "_detailWarp";
		const string detailNoiseSuffix = "_detail";

		PRNG prng = new PRNG (seed);
		PRNG prng2 = new PRNG (seed);
		if (randomize) {
			// warp 1
			var randomizedBiomeWarpNoise = new SimpleNoiseSettings ();
			randomizedBiomeWarpNoise.elevation = prng.Range (0.8f, 3f);
			randomizedBiomeWarpNoise.scale = prng.Range (1f, 3f);
			randomizedBiomeWarpNoise.SetComputeValues (shadingDataCompute, prng2, biomeWarpNoiseSuffix);

			// warp 2
			var randomizedDetailWarpNoise = new SimpleNoiseSettings ();
			randomizedDetailWarpNoise.scale = prng.Range (1f, 3f);
			randomizedDetailWarpNoise.elevation = prng.Range (1f, 5f);
			randomizedDetailWarpNoise.SetComputeValues (shadingDataCompute, prng2, detailWarpNoiseSuffix);

			detailNoise.SetComputeValues (shadingDataCompute, prng2, detailNoiseSuffix);

		} else {
			biomeWarpNoise.SetComputeValues (shadingDataCompute, prng2, biomeWarpNoiseSuffix);
			detailWarpNoise.SetComputeValues (shadingDataCompute, prng2, detailWarpNoiseSuffix);
			detailNoise.SetComputeValues (shadingDataCompute, prng2, detailNoiseSuffix);
		}

	}

	void SetRandomPoints () {
		Random.InitState (seed);

		int randomizedNumPoints = numBiomePoints;
		if (randomize) {
			randomizedNumPoints = Random.Range (15, 50);
		}
		Random.InitState (seed);
		var randomPoints = new Vector4[randomizedNumPoints];
		for (int i = 0; i < randomPoints.Length; i++) {
			var point = Random.onUnitSphere;
			var radius = Mathf.Lerp (radiusMinMax.x, radiusMinMax.y, Random.value);
			randomPoints[i] = new Vector4 (point.x, point.y, point.z, radius);
		}

		ComputeHelper.CreateAndSetBuffer<Vector4> (ref pointBuffer, randomPoints, shadingDataCompute, "points");
		shadingDataCompute.SetInt ("numRandomPoints", randomPoints.Length);
	}

	// Pick craters to be shaded with radial streaks emanating from them 
	void SetCraters (MoonShape moonShape) {
		PRNG random = new PRNG (ejectaRaySeed);
		//int desiredNumCraterRays = random.Range (5, 15);
		//desiredNumCraterRays = 2;

		// Sort craters from largest to smallest
		var sortedCraters = new List<CraterSettings.Crater> (moonShape.craterSettings.cachedCraters);
		sortedCraters.Sort ((a, b) => b.size.CompareTo (a.size));
		int poolSize = Mathf.Clamp ((int) ((sortedCraters.Count - 1) * candidatePoolSize), 1, sortedCraters.Count);
		sortedCraters = sortedCraters.GetRange (0, poolSize);
		random.Shuffle (sortedCraters);

		// Choose craters
		var chosenCraters = new List<CraterSettings.Crater> ();

		for (int i = 0; i < sortedCraters.Count; i++) {
			var currentCrater = sortedCraters[i];

			// Reject those which are too close to already chosen craters as the textures may not overlap
			bool overlapsOtherEjecta = false;
			for (int j = 0; j < chosenCraters.Count; j++) {
				float dst = (currentCrater.centre - chosenCraters[j].centre).magnitude;
				float ejectaRadiusSum = (currentCrater.size + chosenCraters[j].size) * ejectaRaysScale / 2;

				if (dst < ejectaRadiusSum) {
					overlapsOtherEjecta = true;
					break;
				}
			}

			//Debug.DrawRay (currentCrater.centre, currentCrater.centre * 0.2f, (overlapsOtherEjecta) ? Color.red : Color.green);
			if (!overlapsOtherEjecta) {
				chosenCraters.Add (currentCrater);

			}
			if (chosenCraters.Count >= desiredNumCraterRays) {
				break;
			}
		}

		// Set
		var ejectaCraters = new Vector4[chosenCraters.Count];
		for (int i = 0; i < chosenCraters.Count; i++) {
			var crater = chosenCraters[i];
			ejectaCraters[i] = new Vector4 (crater.centre.x, crater.centre.y, crater.centre.z, crater.size * ejectaRaysScale);
			//CustomDebug.DrawSphere (crater.centre, crater.size * ejectaRaysScale / 2, Color.yellow);
		}

		ComputeHelper.CreateAndSetBuffer<Vector4> (ref craterBuffer, ejectaCraters, shadingDataCompute, "ejectaCraters");
		shadingDataCompute.SetInt ("numEjectaCraters", chosenCraters.Count);
	}

	public override void ReleaseBuffers () {
		base.ReleaseBuffers ();
		ComputeHelper.Release (craterBuffer, pointBuffer);
	}

	protected override void OnValidate () {
		base.OnValidate ();
	}
}