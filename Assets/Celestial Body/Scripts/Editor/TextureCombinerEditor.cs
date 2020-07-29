using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (TextureCombiner), true)]
public class TextureCombinerEditor : Editor {

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		var textureCombiner = (TextureCombiner) target;

		if (GUILayout.Button ("Save")) {
			string path = Application.dataPath + "/Resources";
			textureCombiner.SaveTexture (path);
		}

	}

}