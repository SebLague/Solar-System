using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NormalRotTest : MonoBehaviour {

	void Update () {
		Vector3 sphereNormal = transform.up;
		Vector3 normal = transform.GetChild (0).up;
		Vector3 flattenedNormal = (normal - sphereNormal * Vector3.Dot (sphereNormal, normal)).normalized;
		Vector3 axis = Vector3.Cross (sphereNormal, Vector3.up).normalized;

		Debug.DrawRay (transform.position, sphereNormal, Color.green);
		Debug.DrawRay (transform.position, normal, Color.red);
		Debug.DrawRay (transform.position, flattenedNormal, Color.yellow);
		Debug.DrawRay (transform.position, axis, Color.cyan);
		Debug.Log (Vector3.Dot (axis, flattenedNormal));
	}

}