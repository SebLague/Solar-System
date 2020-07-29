using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (ColourTest))]
public class ColourTestEditor : Editor {
	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		if (GUILayout.Button ("Random")) {
			((ColourTest) target).Random ();
		}
	}
}