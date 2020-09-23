using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunShadowCaster : MonoBehaviour {
	Transform track;

	void Start () {
		track = Camera.main?.transform;
	}

	void LateUpdate () {
		if (track) {
			transform.LookAt (track.position);
		}
	}
}