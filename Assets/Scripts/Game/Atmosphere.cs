using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Atmosphere : CustomImageEffect {

	public CelestialBodyGenerator planet;
	[Range (0, 1)]
	public float atmosphereScale = 0.2f;

	public Color color;
	public Vector4 testParams;
	ComputeBuffer buffer;
	Texture2D falloffTex;
	public Gradient falloff;
	public int gradientRes = 10;
	public int numSteps = 10;
	public Texture2D blueNoise;

	public struct Sphere {
		public Vector3 centre;
		public float radius;
		public float waterRadius;

		public static int Size {
			get {
				return sizeof (float) * 5;
			}
		}
	}

	public override Material GetMaterial () {

		// Validate inputs
		if (material == null || material.shader != shader) {
			if (shader == null) {
				shader = Shader.Find ("Unlit/Texture");
			}
			material = new Material (shader);
		}

		// Set
		Sphere sphere = new Sphere () {
			centre = planet.transform.position,
			radius = (1 + atmosphereScale) * planet.BodyScale,
			waterRadius = planet.GetOceanRadius()
		};

		buffer = new ComputeBuffer (1, Sphere.Size);
		buffer.SetData (new Sphere[] { sphere });
		material.SetBuffer ("spheres", buffer);
		material.SetVector ("params", testParams);
		material.SetColor ("_Color", color);
		material.SetFloat ("planetRadius", planet.BodyScale);

		CelestialBodyShading.TextureFromGradient (ref falloffTex, gradientRes, falloff);
		material.SetTexture ("_Falloff", falloffTex);
		material.SetTexture ("_BlueNoise", blueNoise);
		material.SetInt ("numSteps", numSteps);
		return material;
	}

	public override void Release () {
		buffer.Release ();
	}


}