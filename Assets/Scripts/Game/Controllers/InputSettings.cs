using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Settings/Input")]

public class InputSettings : ScriptableObject {

	const float defaultMouseSensitivity = 100;
	const float defaultMouseSmoothing = 0.2f;

	public float mouseSensitivity;
	public float mouseSmoothing;
	public bool lockCursor = true;

	// TODO: find better place to call this from
	public void Begin () {
		LoadSettings ();

		if (lockCursor) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	public void LoadSettings () {
		PlayerPrefs.GetFloat (nameof (mouseSensitivity), defaultMouseSensitivity);
		PlayerPrefs.GetFloat (nameof (mouseSmoothing), defaultMouseSmoothing);

	}

	public void SaveSettings () {
		PlayerPrefs.SetFloat (nameof (mouseSensitivity), mouseSensitivity);
		PlayerPrefs.SetFloat (nameof (mouseSmoothing), mouseSmoothing);
		PlayerPrefs.Save ();
	}

}