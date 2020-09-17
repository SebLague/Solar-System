using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (SceneCamManager))]
public class SceneCamEditor : Editor {

	SceneCamManager manager;

	public override void OnInspectorGUI () {
		DrawDefaultInspector ();

		var activeSceneView = SceneView.lastActiveSceneView;
		var allViews = SceneView.sceneViews;

		if (manager.savedViews.Count > 0) {
			GUILayout.Label ($"Saved views: ({manager.savedViews.Count})");
			int deleteIndex = -1;

			for (int i = 0; i < manager.savedViews.Count; i++) {
				GUILayout.BeginVertical ("GroupBox");
				var savedView = manager.savedViews[i];

				savedView.name = GUILayout.TextField (savedView.name);

				GUILayout.BeginHorizontal ();
				if (GUILayout.Button ("Set Camera View")) {
					Undo.RecordObject (manager, "Set Camera View");
					activeSceneView.pivot = savedView.pivot;
					activeSceneView.rotation = savedView.rotation;
					activeSceneView.size = savedView.size;
				}
				if (GUILayout.Button ("Replace")) {
					Undo.RecordObject (manager, "Replace View");
					savedView.pivot = activeSceneView.pivot;
					savedView.rotation = activeSceneView.rotation;
					savedView.size = activeSceneView.size;
				}
				if (GUILayout.Button ("Delete")) {
					Undo.RecordObject (manager, "Delete View");
					deleteIndex = i;
				}
				GUILayout.EndHorizontal ();
				GUILayout.EndVertical ();
			}

			if (deleteIndex != -1) {
				manager.savedViews.RemoveAt (deleteIndex);
			}

			GUILayout.Space (15);
		}

		foreach (var v in allViews) {
			//GUILayout.Label (v.ToString ());
		}

		if (GUILayout.Button ("Save Current View")) {
			Undo.RecordObject (manager, "Save View");
			var newView = new SceneCamManager.SavedView ();
			newView.name = $"View ({manager.savedViews.Count})";
			newView.pivot = activeSceneView.pivot;
			newView.rotation = activeSceneView.rotation;
			newView.size = activeSceneView.size;
			manager.savedViews.Add (newView);
		}
	}

	void OnEnable () {
		manager = (SceneCamManager) target;

		if (manager.savedViews == null) {
			manager.savedViews = new List<SceneCamManager.SavedView> ();
		}
	}

}