using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

[CreateAssetMenu (menuName = "Celestial Body/Textures/Noise")]
public class NoiseGenerator : TextureGenerator {

	public SimpleNoiseSettings noiseSettings;
	public SimpleNoiseSettings warpNoiseSettings;
	[Range (0, 1)]
	public float valueFloor;
	public bool normalize;
	ComputeBuffer minMaxBuffer;

	protected override void Run () {
		var prng = new PRNG (seed);
		var offset = new Vector4 (prng.Value (), prng.Value (), prng.Value (), prng.Value ()) * 10;

		ComputeHelper.CreateStructuredBuffer<int> (ref minMaxBuffer, 2);
		minMaxBuffer.SetData (new int[] { int.MaxValue, 0 });
		compute.SetBuffer (0, "minMax", minMaxBuffer);
		compute.SetBuffer (1, "minMax", minMaxBuffer);

		int threadGroupSize = ComputeHelper.GetThreadGroupSizes (compute, 0).x;
		int numThreadGroups = Mathf.CeilToInt ((float) textureSize / threadGroupSize);
		compute.SetVector ("offset", offset);
		compute.SetTexture (0, "Result", renderTexture);
		noiseSettings.SetComputeValues (compute, prng, "_simple");
		warpNoiseSettings.SetComputeValues (compute, prng, "_warp");

		compute.SetInt ("resolution", (int) textureSize);
		compute.SetFloat ("valueFloor", valueFloor);
		compute.Dispatch (0, numThreadGroups, numThreadGroups, 1);

		// Normalize
		if (normalize) {
			compute.SetTexture (1, "Result", renderTexture);
			compute.Dispatch (1, numThreadGroups, numThreadGroups, 1);
		}

		ComputeHelper.Release (minMaxBuffer);
	}

}