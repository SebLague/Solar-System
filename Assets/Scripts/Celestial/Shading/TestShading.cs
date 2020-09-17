using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Shading/Test")]
public class TestShading : CelestialBodyShading {

	public Color colorA = Color.black;
	public Color colorB = Color.white;
	public Vector2 remapMinMax = new Vector2 (0, 1);

	public override void SetTerrainProperties (Material material, Vector2 heightMinMax, float bodyScale) {
		material.SetColor ("_ColorA", colorA);
		material.SetColor ("_ColorB", colorB);
		material.SetVector ("heightMinMax", heightMinMax);
		material.SetVector ("remapMinMax", remapMinMax);
	}
}