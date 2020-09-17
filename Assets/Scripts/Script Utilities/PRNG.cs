using System;
using System.Collections.Generic;
using UnityEngine;

public class PRNG {

	public enum Weight { None, Lower, Upper, Centre, Ends }
	System.Random prng;
	int seed;

	public int Seed {
		get {
			return seed;
		}
	}

	public PRNG (int seed) {
		this.seed = seed;
		prng = new System.Random (this.seed);
	}

	public PRNG (string seed) {
		this.seed = seed.GetHashCode ();
		prng = new System.Random (this.seed);
	}

	public PRNG () {
		prng = new System.Random ();
	}

	/// Returns a random integer value [min, max)
	public int Range (int min, int max) {
		return prng.Next (min, max);
	}

	/// Returns a random float value [min, max)
	public float Range (float min, float max) {
		return Mathf.Lerp (min, max, (float) prng.NextDouble ());
	}

	// Returns a vector4 where each component is a random number in range [min, max)
	public Vector4 RangeVector4 (float minInclusive, float maxExclusive) {
		Vector4 vector = Vector4.zero;
		for (int i = 0; i < 4; i++) {
			vector[i] = Range (minInclusive, maxExclusive);
		}
		return vector;
	}

	/// Random value [0, 1]
	public float Value () {
		// According to stackoverflow this should technically allow the random value to equal 1 
		const double maxExclusive = 1.0000000004656612875245796924106;
		return (float) (prng.NextDouble () * maxExclusive);
	}

	/// Random value [0, 1]
	/// Output is increasingly biased toward 1 as biasStrength goes from 0 to 1
	public float ValueBiasUpper (float biasStrength) {
		return 1 - ValueBiasLower (biasStrength);
	}

	/// Random value [0, 1]
	/// Output is increasingly biased toward 0 as biasStrength goes from 0 to 1
	public float ValueBiasLower (float biasStrength) {
		float t = Value ();

		// Avoid possible division by zero
		if (biasStrength == 1) {
			return 0;
		}

		// Remap strength for nicer input -> output relationship
		float k = Mathf.Clamp01 (1 - biasStrength);
		k = k * k * k - 1;

		// Thanks to www.shadertoy.com/view/Xd2yRd
		return Mathf.Clamp01 ((t + t * k) / (t * k + 1));
	}

	/// Random value [0, 1]
	/// Output is increasingly biased toward the extremes (0 or 1) as biasStrength goes from 0 to 1
	public float ValueBiasExtremes (float biasStrength) {
		float t = ValueBiasLower (biasStrength);
		return (Value () < 0.5f) ? t : 1 - t;
	}

	/// Random value [0, 1]
	/// Output is increasingly biased toward 0.5 as biasStrength goes from 0 to 1
	public float ValueBiasCentre (float biasStrength) {
		float t = ValueBiasLower (biasStrength);
		return 0.5f + t * 0.5f * Sign ();
	}

	// ---- Signed versions [-1, 1] ---

	/// Random value [-1, 1]
	public float SignedValue () {
		return Value () * 2 - 1;
	}

	/// Random value [-1, 1]
	/// Output is increasingly biased toward the extremes (-1 or 1) as biasStrength goes from 0 to 1
	public float SignedValueBiasExtremes (float biasStrength) {
		return ValueBiasExtremes (biasStrength) * 2 - 1;
	}

	/// Random value [-1, 1]
	/// Output is increasingly biased toward 0 as biasStrength goes from 0 to 1
	public float SignedValueBiasCentre (float biasStrength) {
		return ValueBiasCentre (biasStrength) * 2 - 1;
	}

	// Returns a weighted value in range [min, max)
	public float WeightedRange (float min, float max, Weight weight, int weightStrength = 4) {
		float value01 = WeightedValue (weight, weightStrength);
		return Mathf.Lerp (min, max, value01);
	}

	// Returns a weighted value in range [-1, 1)
	public float WeightedSignedValue (Weight weight, int weightStrength = 4) {
		return WeightedValue (weight, weightStrength) * 2 - 1;
	}

	// Returns a weighted value in range [0, 1)
	public float WeightedValue (Weight weight, int weightStrength = 4) {

		if (weight == Weight.None) {
			return Value ();
		}

		float smallestValue = Value ();
		for (int i = 0; i < weightStrength; i++) {
			smallestValue = Mathf.Min (smallestValue, Value ());
		}

		switch (weight) {
			case Weight.Lower: // Bias towards smaller values
				return smallestValue;
			case Weight.Upper: // Bias towards larger values
				return 1 - smallestValue;
			case Weight.Centre: // Bias towards middle values
				return 0.5f + smallestValue * 0.5f * Sign ();
			case Weight.Ends: // Bias towards smaller and larger values
				return (Value () < 0.5f) ? smallestValue : 1 - smallestValue;
			default:
				Debug.LogError ("Missing weight implementation");
				return 0;
		}
	}

	// Returns the smallest of n random numbers between 0 and 1
	public float SmallestRandom01 (int n) {
		float smallest = 1;
		for (int i = 0; i < n; i++) {
			smallest = Mathf.Min (smallest, Value ());
		}
		return smallest;
	}

	// Returns the largest of n random numbers between 0 and 1
	public float LargestRandom01 (int n) {
		float largest = 0;
		for (int i = 0; i < n; i++) {
			largest = Mathf.Max (largest, Value ());
		}
		return largest;
	}

	// Returns the value closest to 0.5 out of n random numbers between 0 and 1
	public float CentredRandom01 (int n) {
		float mostCentredValue = 0;
		for (int i = 0; i < n; i++) {
			float value = Value ();
			if (Mathf.Abs (value - 0.5f) < Mathf.Abs (mostCentredValue - 0.5f)) {
				mostCentredValue = value;
			}
		}
		return mostCentredValue;
	}

	public int Sign () {
		return (prng.NextDouble () > 0.5) ? 1 : -1;
	}

	public int NextInt () {
		return prng.Next ();
	}

	public Vector3 JiggleVector3 (float weightX, float weightY, float weightZ) {
		return new Vector3 (SignedValue () * weightX, SignedValue () * weightY, SignedValue () * weightZ);
	}

	// Colours
	public Color ColorHSV (float saturationMin, float saturationMax, float valueMin, float valueMax) {
		float hue = Value ();
		float saturation = Range (saturationMin, saturationMax);
		float value = Range (valueMin, valueMax);
		return Color.HSVToRGB (hue, saturation, value);
	}

	public Color ColorGreyscale (float valueMin, float valueMax) {
		float value = Range (valueMin, valueMax);
		return new Color (value, value, value);
	}
	// Elements
	public T RandomElement<T> (T[] array) {
		return array[Range (0, array.Length)];
	}

	public void Shuffle<T> (T[] array) {
		int n = array.Length;
		for (int i = 0; i < n - 1; i++) {
			int j = prng.Next (i, n);
			T temp = array[j];
			array[j] = array[i];
			array[i] = temp;
		}
	}

	public void Shuffle<T> (List<T> list) {
		int n = list.Count;
		for (int i = 0; i < n - 1; i++) {
			int j = prng.Next (i, n);
			T temp = list[j];
			list[j] = list[i];
			list[i] = temp;
		}
	}

}