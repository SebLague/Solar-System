using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (TextureViewer), true)]
public class TextureEditor : Editor {

	Editor generatorEditor;
	bool generatorFoldout;

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		var textureViewer = (TextureViewer) target;

		if (GUILayout.Button ("Generate")) {
			textureViewer.UpdateTexture ();
		}

		if (GUILayout.Button ("Save")) {
			string path = Application.dataPath + "/Resources";
			textureViewer.SaveTexture (path);
		}

		if (textureViewer.generator) {
			bool settingsUpdated = DrawSettingsEditor (textureViewer.generator, ref generatorFoldout, ref generatorEditor);
			if (settingsUpdated) {
				textureViewer.UpdateTexture ();
			}
		}
	}

	bool DrawSettingsEditor (Object settings, ref bool foldout, ref Editor editor) {
		if (settings != null) {
			foldout = EditorGUILayout.InspectorTitlebar (foldout, settings);
			if (foldout) {
				using (var check = new EditorGUI.ChangeCheckScope ()) {
					CreateCachedEditor (settings, null, ref editor);
					editor.OnInspectorGUI ();
					if (check.changed) {
						return true;
					}
				}
			}
		}
		return false;
	}

	private void OnEnable () {
		generatorFoldout = EditorPrefs.GetBool (nameof (generatorFoldout), false);
	}

	void OnDisable () {
		EditorPrefs.SetBool (nameof (generatorFoldout), generatorFoldout);
	}
}