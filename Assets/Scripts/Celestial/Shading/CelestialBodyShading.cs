using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
	Responsible for the shading of a celestial body.
	This is paired with a specific CelestialBodyShape.
*/

public abstract class CelestialBodyShading : ScriptableObject {

	public event System.Action OnSettingChanged;

	public bool randomize;
	public int seed;

	public Material terrainMaterial = null;
	public bool hasAtmosphere;
	public AtmosphereSettings atmosphereSettings;
	public bool hasOcean;
	[Range (0, 1)]
	public float oceanLevel;
	public OceanSettings oceanSettings;

	public ComputeShader shadingDataCompute;

	protected Vector4[] cachedShadingData;
	ComputeBuffer shadingBuffer;

	// 
	public virtual void Initialize (CelestialBodyShape shape) { }

	// Generate Vector4[] of shading data. This is stored in mesh uvs and used to help shade the body
	public Vector4[] GenerateShadingData (ComputeBuffer vertexBuffer) {
		int numVertices = vertexBuffer.count;
		Vector4[] shadingData = new Vector4[numVertices];

		if (shadingDataCompute) {
			// Set data
			SetShadingDataComputeProperties ();

			shadingDataCompute.SetInt ("numVertices", numVertices);
			shadingDataCompute.SetBuffer (0, "vertices", vertexBuffer);
			ComputeHelper.CreateAndSetBuffer<Vector4> (ref shadingBuffer, numVertices, shadingDataCompute, "shadingData");

			// Run
			ComputeHelper.Run (shadingDataCompute, numVertices);

			// Get data
			shadingBuffer.GetData (shadingData);
		}

		cachedShadingData = shadingData;
		return shadingData;
	}

	// Set shading properties on terrain
	public virtual void SetTerrainProperties (Material material, Vector2 heightMinMax, float bodyScale) {

	}

	public virtual void SetOceanProperties (Material oceanMaterial) {
		if (oceanSettings) {
			oceanSettings.SetProperties (oceanMaterial, seed, randomize);
		}
	}

	// Override this to set properties on the shadingDataCompute before it is run
	protected virtual void SetShadingDataComputeProperties () {

	}

	public virtual void ReleaseBuffers () {
		ComputeHelper.Release (shadingBuffer);
	}

	public static void TextureFromGradient (ref Texture2D texture, int width, Gradient gradient, FilterMode filterMode = FilterMode.Bilinear) {
		if (texture == null) {
			texture = new Texture2D (width, 1);
		} else if (texture.width != width) {
			texture.Resize (width, 1);
		}
		if (gradient == null) {
			gradient = new Gradient ();
			gradient.SetKeys (
				new GradientColorKey[] { new GradientColorKey (Color.black, 0), new GradientColorKey (Color.black, 1) },
				new GradientAlphaKey[] { new GradientAlphaKey (1, 0), new GradientAlphaKey (1, 1) }
			);
		}
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = filterMode;

		Color[] cols = new Color[width];
		for (int i = 0; i < cols.Length; i++) {
			float t = i / (cols.Length - 1f);
			cols[i] = gradient.Evaluate (t);
		}
		texture.SetPixels (cols);
		texture.Apply ();
	}

	protected virtual void OnValidate () {
		/*
		Shader activeShader = (shader) ? shader : Shader.Find ("Unlit/Color");
		if (material == null || material.shader != activeShader) {
		    if (material == null) {
		        material = new Material (activeShader);
		    } else {
		        material.shader = activeShader;
		    }
		}
		*/
		if (OnSettingChanged != null) {
			OnSettingChanged ();
		}
	}
}