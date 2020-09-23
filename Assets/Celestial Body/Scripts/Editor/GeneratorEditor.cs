using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (CelestialBodyGenerator))]
public class GeneratorEditor : Editor {

	CelestialBodyGenerator generator;
	Editor shapeEditor;
	Editor shadingEditor;

	bool shapeFoldout;
	bool shadingFoldout;

	public override void OnInspectorGUI () {
		using (var check = new EditorGUI.ChangeCheckScope ()) {
			DrawDefaultInspector ();
			if (check.changed) {
				Regenerate ();
			}
		}

		if (GUILayout.Button ("Generate")) {
			Regenerate ();
		}
		if (GUILayout.Button ("Randomize Shading")) {
			var prng = new System.Random ();
			generator.body.shading.randomize = true;
			generator.body.shading.seed = prng.Next (-10000, 10000);
			Regenerate ();
		}

		if (GUILayout.Button ("Randomize Shape")) {
			var prng = new System.Random ();
			generator.body.shape.randomize = true;
			generator.body.shape.seed = prng.Next (-10000, 10000);
			Regenerate ();
		}

		if (GUILayout.Button ("Randomize Both")) {
			var prng = new System.Random ();
			generator.body.shading.randomize = true;
			generator.body.shape.randomize = true;
			generator.body.shape.seed = prng.Next (-10000, 10000);
			generator.body.shading.seed = prng.Next (-10000, 10000);
			Regenerate ();
		}

		bool randomized = generator.body.shading.randomize || generator.body.shape.randomize;
		randomized |= generator.body.shading.seed != 0 || generator.body.shape.seed != 0;
		using (new EditorGUI.DisabledGroupScope (!randomized)) {
			if (GUILayout.Button ("Reset Randomization")) {
				var prng = new System.Random ();
				generator.body.shading.randomize = false;
				generator.body.shape.randomize = false;
				generator.body.shape.seed = 0;
				generator.body.shading.seed = 0;
				Regenerate ();
			}
		}

		// Draw shape/shading object editors
		DrawSettingsEditor (generator.body.shape, ref shapeFoldout, ref shapeEditor);
		DrawSettingsEditor (generator.body.shading, ref shadingFoldout, ref shadingEditor);

		SaveState ();
	}

	void Regenerate () {
		generator.OnShapeSettingChanged ();
		generator.OnShadingNoiseSettingChanged ();
		EditorApplication.QueuePlayerLoopUpdate ();
	}

	void DrawSettingsEditor (Object settings, ref bool foldout, ref Editor editor) {
		if (settings != null) {
			foldout = EditorGUILayout.InspectorTitlebar (foldout, settings);
			if (foldout) {
				CreateCachedEditor (settings, null, ref editor);
				editor.OnInspectorGUI ();
			}
		}
	}

	private void OnEnable () {
		shapeFoldout = EditorPrefs.GetBool (nameof (shapeFoldout), false);
		shadingFoldout = EditorPrefs.GetBool (nameof (shadingFoldout), false);
		generator = (CelestialBodyGenerator) target;
	}

	void SaveState () {
		EditorPrefs.SetBool (nameof (shapeFoldout), shapeFoldout);
		EditorPrefs.SetBool (nameof (shadingFoldout), shadingFoldout);
	}
}