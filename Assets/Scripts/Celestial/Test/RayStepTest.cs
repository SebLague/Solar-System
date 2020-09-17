using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayStepTest : MonoBehaviour {

	public float skin = 0;
	public int numSteps = 3;
	public float displayRad = 0.1f;

	void OnDrawGizmos () {
		Vector3 start = Vector3.right * -5;
		Vector3 end = Vector3.up * 7 + Vector3.right * 3;
		Gizmos.color = Color.red;
		Gizmos.DrawLine (start, end);

		float dst = (start - end).magnitude;
		Vector3 dir = (end - start).normalized;
		float stepSize = (dst - skin * 2) / (numSteps - 1f);
		Vector3 pos = start + dir * skin;

		Gizmos.color = Color.black;
		for (int i = 0; i < numSteps; i++) {
			Gizmos.DrawSphere (pos, displayRad);
			pos += dir * stepSize;
		}
	}
}