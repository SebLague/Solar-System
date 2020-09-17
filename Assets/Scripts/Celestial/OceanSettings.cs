using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Ocean")]
public class OceanSettings : ScriptableObject {
	public float depthMultiplier = 10;
	public float alphaMultiplier = 70;
	public Color colA;
	public Color colB;
	public Color specularCol = Color.white;
	[Header ("Waves")]
	public Texture2D waveNormalA;
	public Texture2D waveNormalB;
	[Range (0, 1)]
	public float waveStrength = 0.15f;
	public float waveScale = 15;
	public float waveSpeed = 0.5f;

	//[Header("")]
	[Range (0, 1)]
	public float smoothness = 0.92f;
	public Vector4 testParams;

	public void SetProperties (Material material, int seed, bool randomize) {
		material.SetFloat ("depthMultiplier", depthMultiplier);
		material.SetFloat ("alphaMultiplier", alphaMultiplier);

		material.SetTexture ("waveNormalA", waveNormalA);
		material.SetTexture ("waveNormalB", waveNormalB);
		material.SetFloat ("waveStrength", waveStrength);
		material.SetFloat ("waveNormalScale", waveScale);
		material.SetFloat ("waveSpeed", waveSpeed);
		material.SetFloat ("smoothness", smoothness);
		material.SetVector ("params", testParams);

		if (randomize) {
			var random = new PRNG (seed);
			var randomColA = Color.HSVToRGB (random.Value (), random.Range (0.6f, 0.8f), random.Range (0.65f, 1));
			var randomColB = ColourHelper.TweakHSV (randomColA,
				random.SignedValue() * 0.2f,
				random.SignedValue() * 0.2f,
				random.Range (-0.5f, -0.4f)
			);

			material.SetColor ("colA", randomColA);
			material.SetColor ("colB", randomColB);
			material.SetColor ("specularCol", Color.white);
		} else {
			material.SetColor ("colA", colA);
			material.SetColor ("colB", colB);
			material.SetColor ("specularCol", specularCol);
		}
	}
}