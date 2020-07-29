using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityDebugUI : MonoBehaviour {
	public TMPro.TMP_Text info;
	bool show;

	void Update () {
		if (Input.GetKeyDown (KeyCode.Tab)) {
			show = !show;
		}

		info.text = "";

		if (show) {
			var grav = GetGravityInfo (Camera.main.transform.position);
			for (int i = 0; i < grav.Length; i++) {
				info.text += grav[i] + "\n";
			}
		}
	}

	static string[] GetGravityInfo (Vector3 point, CelestialBody ignore = null) {
		CelestialBody[] bodies = GameObject.FindObjectsOfType<CelestialBody> ();
		Vector3 totalAcc = Vector3.zero;

		// gravity
		var forceAndName = new List<FloatAndString> ();
		foreach (CelestialBody body in bodies) {
			if (body != ignore) {
				var offsetToBody = body.Position - point;
				var sqrDst = offsetToBody.sqrMagnitude;
				float dst = Mathf.Sqrt (sqrDst);
				var dirToBody = offsetToBody / Mathf.Sqrt (sqrDst);
				var acceleration = Universe.gravitationalConstant * body.mass / sqrDst;
				totalAcc += dirToBody * acceleration;
				forceAndName.Add (new FloatAndString () { floatVal = acceleration, stringVal = body.gameObject.name });

			}
		}
		forceAndName.Sort ((a, b) => (b.floatVal.CompareTo (a.floatVal)));
		string[] info = new string[forceAndName.Count + 1];
		//info[0] = $"acceleration: {totalAcc.magnitude:0.00})";
		info[0] = "Acceleration due to bodies: (m/s^2)";
		for (int i = 0; i < forceAndName.Count; i++) {
			info[i + 1] = $"{forceAndName[i].stringVal}: {forceAndName[i].floatVal:0.00}".Replace (",", ".");
		}
		return info;
	}

	struct FloatAndString {
		public float floatVal;
		public string stringVal;
	}
}