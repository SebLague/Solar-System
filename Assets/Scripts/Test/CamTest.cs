using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CamTest : MonoBehaviour {

	public bool orient;
	public bool setTimestep;
	public float physicsStep = 0.2f;
	public Vector3 initial;

	void Start () {
		GetComponent<Rigidbody> ().velocity = initial;
		if (setTimestep) {

		}
	}

	void FixedUpdate () {
		Debug.Log (GetComponent<Rigidbody> ().velocity);
		GetComponent<Rigidbody> ().position += GetComponent<Rigidbody> ().velocity * Time.deltaTime;
	}

}