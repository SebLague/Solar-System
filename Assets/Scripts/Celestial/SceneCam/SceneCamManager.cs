using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCamManager : MonoBehaviour {

	[HideInInspector]
	public List<SavedView> savedViews;

	[System.Serializable]
	public class SavedView {
		public string name;
		public float size;
		public Vector3 pivot;
		public Quaternion rotation;
	}
}