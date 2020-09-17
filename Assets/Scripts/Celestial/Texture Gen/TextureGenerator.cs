using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public abstract class TextureGenerator : ScriptableObject {
	public enum TextureSize { u128 = 128, u256 = 256, u512 = 512, u1024 = 1024, u2048 = 2048, u4096 = 4096 }

	[Header ("Texture Settings")]
	public string textureName = "Unnamed";
	public TextureSize textureSize = TextureSize.u512;
	public FilterMode filterMode = FilterMode.Bilinear;
	public GraphicsFormat format = GraphicsFormat.R8G8B8A8_UNorm;
	public bool useMips = true;

	[Header ("Compute Settings")]
	public ComputeShader compute;
	public int seed;
	public Vector4 testParams;

	[SerializeField, HideInInspector]
	protected RenderTexture renderTexture;

	public RenderTexture GenerateTexture () {
		int resolution = (int) textureSize;
		CreateTexture (ref renderTexture, resolution, textureName);

		if (compute) {
			compute.SetVector ("params", testParams);
			Run ();

			if (useMips) {
				renderTexture.GenerateMips ();
			}
		}

		return renderTexture;
	}

	protected abstract void Run ();

	void CreateTexture (ref RenderTexture texture, int resolution, string name) {
		if (texture == null || !texture.IsCreated () || texture.width != resolution || texture.height != resolution || texture.useMipMap != useMips || texture.graphicsFormat != format) {
			if (texture != null) {
				texture.Release ();
			}
			texture = new RenderTexture (resolution, resolution, 0);
			texture.graphicsFormat = format;
			texture.enableRandomWrite = true;

			texture.autoGenerateMips = false;
			texture.useMipMap = useMips;
			texture.Create ();
		}
		texture.wrapMode = TextureWrapMode.Repeat;
		texture.filterMode = filterMode;
		texture.name = name;
	}

	void OnDestroy () {
		if (renderTexture != null) {
			renderTexture.Release ();
		}
	}

}