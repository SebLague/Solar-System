using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Celestial Body/Textures/Spots")]
public class SpotsTexture : TextureGenerator {

	[Header ("Spot Settings")]
	public float fadeDst = 1;
	public float smoothing;
	[Range (0, 1)]
	public float background = 0;
	public Layer[] layers;

	public bool blur;
	public ComputeShader blurCompute;
	[Range (1, 20)]
	public int blurSize = 1;
	public bool invertAlpha;
	public float initialRadius;
	public float radiusMultiplier;

	protected override void Run () {
		const float dstScale = 0.001f;
		var spots = new List<Vector4> ();
		Random.InitState (seed);

		int layerIndex = 0;
		float currentRadius = initialRadius * dstScale;
		foreach (Layer layer in layers) {
			float cellSize = 1f / layer.numCellsPerAxis;
			//float currentRadius = layer.radius * dstScale;
			float alpha = Mathf.Lerp (0, 1 - background, layer.alpha);
			layerIndex++;
			alpha = layerIndex / (float) layers.Length;
			if (invertAlpha) {
				alpha = 1 - alpha;
			}
			Debug.Log (alpha + "  " + currentRadius);

			for (int x = 0; x < layer.numCellsPerAxis; x++) {
				for (int y = 0; y < layer.numCellsPerAxis; y++) {
					Vector2 randomOffset = new Vector2 (Random.value, Random.value) * cellSize;
					Vector2 cellCorner = new Vector2 (x, y) * cellSize;
					Vector2 pos = cellCorner + randomOffset;

					var spot = new Vector4 (pos.x, pos.y, currentRadius, alpha);
					spots.Add (spot);

					// mirror
					float mirrorX = (pos.x < .5f) ? pos.x + 1 : pos.x - 1;
					float mirrorY = (pos.y < .5f) ? pos.y + 1 : pos.y - 1;
					float mirrorDstX = Mathf.Min (Mathf.Abs (mirrorX - 1), Mathf.Abs (mirrorX));
					float mirrorDstY = Mathf.Min (Mathf.Abs (mirrorY - 1), Mathf.Abs (mirrorY));

					if (mirrorDstX < currentRadius) {
						spots.Add (new Vector4 (mirrorX, pos.y, currentRadius, alpha));
					}
					if (mirrorDstY < currentRadius) {
						spots.Add (new Vector4 (pos.x, mirrorY, currentRadius, alpha));
					}
					if (mirrorDstX < currentRadius && mirrorDstY < currentRadius) {
						spots.Add (new Vector4 (mirrorX, mirrorY, currentRadius, alpha));
					}
				}
			}
			currentRadius *= radiusMultiplier;
		}

		if (spots.Count > 0) {
			var spotBuffer = ComputeHelper.CreateAndSetBuffer<Vector4> (spots.ToArray (), compute, "spots");

			compute.SetTexture (0, "Result", renderTexture);
			compute.SetInt ("numSpots", spots.Count);
			compute.SetInt ("resolution", renderTexture.width);
			compute.SetFloat ("fadeDst", fadeDst * dstScale);
			compute.SetFloat ("smoothing", smoothing);
			compute.SetFloat ("background", background);
			ComputeHelper.Run (compute, renderTexture.width, renderTexture.height);

			ComputeHelper.Release (spotBuffer);

			if (blur && blurCompute) {
				var unblurredTexture = new RenderTexture (renderTexture);
				Graphics.CopyTexture (renderTexture, unblurredTexture);
				blurCompute.SetTexture (0, "SourceTex", unblurredTexture);
				blurCompute.SetTexture (0, "Result", renderTexture);
				blurCompute.SetInt ("blurSize", blurSize);
				blurCompute.SetInt ("textureSize", (int) textureSize);
				ComputeHelper.Run (blurCompute, renderTexture.width, renderTexture.height);
				unblurredTexture.Release ();
			}
		} else {
			Debug.Log ("No points set in layers");
		}

	}

	[System.Serializable]
	public class Layer {
		public int numCellsPerAxis = 10;
		public float radius = 3;
		[Range (0, 1)]
		public float alpha = 1;

	}

}