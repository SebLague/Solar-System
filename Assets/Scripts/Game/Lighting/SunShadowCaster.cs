using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunShadowCaster : MonoBehaviour {
    Transform track;

    void Start () {
        track = Camera.main.transform;
    }

    void LateUpdate () {
        transform.LookAt (track.position);
    }
}