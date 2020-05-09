using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugHelper {

    public static void HandleEditorInput (bool lockCursor) {
        if (Input.GetKeyDown (KeyCode.Escape)) {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        if (Input.GetMouseButtonDown (0) && lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        if (Input.GetKeyDown (KeyCode.P)) {
            Debug.Break ();
        }
    }
}