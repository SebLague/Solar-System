using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TextureHelper {

	public static void TextureFromGradient (Gradient gradient, int width, ref Texture2D texture) {
		if (texture == null || texture.width != width || texture.height != 1) {
			texture = new Texture2D (width, 1);
			texture.filterMode = FilterMode.Bilinear;
			texture.wrapMode = TextureWrapMode.Clamp;
		}

		Color[] colours = new Color[width];
		for (int i = 0; i < width; i++) {
			float t = i / (width - 1f);
			colours[i] = gradient.Evaluate (t);
		}
		texture.SetPixels (colours);
		texture.Apply ();
	}
}