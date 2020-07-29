using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (GravityObject), true)]
[CanEditMultipleObjects]
public class GravityBodyEditor : Editor {
	GravityObject gravityObject;
	bool showDebugInfo;

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		EditorGUILayout.Space (10);
		EditorGUILayout.LabelField ("Debug", EditorStyles.boldLabel);
		showDebugInfo = EditorGUILayout.Foldout (showDebugInfo, "Debug info");
		if (showDebugInfo) {
			string[] gravityInfo = GetGravityInfo (gravityObject.transform.position, gravityObject as CelestialBody);
			for (int i = 0; i < gravityInfo.Length; i++) {
				EditorGUILayout.LabelField (gravityInfo[i]);
			}
		}
	}

	void OnEnable () {
		gravityObject = (GravityObject) target;
		showDebugInfo = EditorPrefs.GetBool (gravityObject.gameObject.name + nameof (showDebugInfo), false);
	}

	void OnDisable () {
		if (gravityObject) {
			EditorPrefs.SetBool (gravityObject.gameObject.name + nameof (showDebugInfo), showDebugInfo);
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
		info[0] = $"acc: {totalAcc} (mag = {totalAcc.magnitude})";
		for (int i = 0; i < forceAndName.Count; i++) {
			info[i + 1] = $"acceleration due to {forceAndName[i].stringVal}: {forceAndName[i].floatVal}";
		}
		return info;
	}

	struct FloatAndString {
		public float floatVal;
		public string stringVal;
	}
}