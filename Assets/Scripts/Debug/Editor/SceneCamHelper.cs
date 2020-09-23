using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class SceneCamHelper {

    [MenuItem ("Edit/Camera/SetPivot")]
    static void SetCam () {
        SceneView.lastActiveSceneView.FrameSelected ();
        SceneView.lastActiveSceneView.size = 2.5f;
    }

}