using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SettingsMenu : MonoBehaviour {

	bool inMenu;
	public GameObject menuPanel;
	public InputSettings inputSettings;
	public TMP_InputField mouseSensitivity;
	public UnityEngine.UI.Slider mouseSmoothingSlider;

	void Awake () {
		menuPanel.SetActive (false);
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape) || Input.GetKeyDown (KeyCode.P)) {
			if (inMenu) {
				CloseMenu ();
			} else {
				OpenMenu ();
			}
		}
	}

	public void OpenMenu () {
		inMenu = true;
		Time.timeScale = 0;
		menuPanel.SetActive (true);

		mouseSensitivity.text = inputSettings.mouseSensitivity + "";
		mouseSmoothingSlider.value = inputSettings.mouseSmoothing;

		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	public void CloseMenu () {
		inMenu = false;
		Time.timeScale = 1;
		menuPanel.SetActive (false);

		int sensitivity;
		if (int.TryParse (mouseSensitivity.text, out sensitivity)) {
			inputSettings.mouseSensitivity = sensitivity;
		}

		inputSettings.mouseSmoothing = mouseSmoothingSlider.value;

		inputSettings.SaveSettings ();

		if (inputSettings.lockCursor) {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
		}
	}
}