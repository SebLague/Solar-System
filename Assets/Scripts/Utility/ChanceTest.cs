using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ChanceTest : MonoBehaviour {
    public Chance chance;
    public int total;

    void Update () {
        if (Input.GetKeyDown (KeyCode.Space)) {
            chance = new Chance (new System.Random ());
            total++;
            if (chance.Percent (10)) {
                Debug.Log ("1");
            }
            if (chance.Percent (70)) {
                Debug.Log ("7");
            }
            if (chance.Percent (20)) {
                Debug.Log ("2");
            }

        }
    }
}

public class Chance {
    float value;

    public Chance (System.Random prng) {
        value = (float) prng.NextDouble ();
    }

    public Chance (PRNG prng) {
        value = prng.Value ();
    }

    public bool Percent (float percent) {
        if (value <= 0) {
            return false;
        }

        float t = percent / 100f;
        value -= t;
        return value <= 0;
    }
}