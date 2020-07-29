using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTest : MonoBehaviour {

	public TMPro.TMP_Text fpsUI;
	public int numFrames = 5;
	float[] dts;
	int i = 0;

	// Start is called before the first frame update
	void Start () {
		dts = new float[numFrames];
	}

	// Update is called once per frame
	void Update () {
		dts[i] = Time.deltaTime * 1000;
		i++;
		i %= numFrames;
		if (Time.frameCount >= numFrames) {
			float sum = 0;
			float min = float.MaxValue;
			float max = float.MinValue;
			for (int i = 0; i < numFrames; i++) {
				sum += dts[i];
				min = Mathf.Min (min, dts[i]);
				max = Mathf.Max (max, dts[i]);
			}
			float avg = sum / numFrames;

			fpsUI.text = "FPS: " + ToFPS (avg);
			fpsUI.text += "\nBest: " + ToFPS (min);
			fpsUI.text += "\nWorst: " + ToFPS (max);
			fpsUI.text += "\nAvg dt: " + avg + " ms";
		}
	}

	string ToFPS (float millis) {
		float fps = 1000 / millis;
		return (((int) fps * 10000) / 10000f) + "";
	}
}