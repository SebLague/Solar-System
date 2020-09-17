using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Normalizer {

    public static void Normalize (System.Func<int, float> getValue, System.Action<int, float> setValue, int length) {
        float min = float.PositiveInfinity;
        float max = float.NegativeInfinity;

        for (int i = 0; i < length; i++) {
            float value = getValue (i);
            min = System.Math.Min (min, value);
            max = System.Math.Max (max, value);
        }
        for (int i = 0; i < length; i++) {
            setValue (i, Mathf.InverseLerp (min, max, getValue (i)));
        }
    }

}