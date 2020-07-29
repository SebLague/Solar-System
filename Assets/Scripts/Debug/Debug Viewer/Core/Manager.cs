using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Visualization.MeshGeneration;

namespace Visualization {

	public enum Style { Diffuse, Unlit, UnlitAlpha }

	public static class Manager {

		static readonly string[] shaderPaths = {
			"Visualizer/Diffuse",
			"Visualizer/Unlit",
			"Visualizer/UnlitColorAlpha"
		};

		static Material[] materials;
		static MaterialPropertyBlock materialProperties;

		// Cached meshes:
		// These are meshes that don't change, in contrast to dynamic meshes (like an arc, where the angle can change)
		// As such, they only need to be generated once, and reused as needed.
		public static Mesh sphereMesh;
		public static Mesh cylinderMesh;

		static Queue<Mesh> inactiveMeshes;
		static List<VisualElement> drawList;

		static int lastFrameInputReceived;

		static Manager () {
			Camera.onPreCull -= Draw;
			Camera.onPreCull += Draw;

			Init ();
		}

		static void Init () {
			if (sphereMesh == null) {
				inactiveMeshes = new Queue<Mesh> ();
				materialProperties = new MaterialPropertyBlock ();
				drawList = new List<VisualElement> ();

				// Generate and cache primitive meshes
				sphereMesh = new Mesh ();
				cylinderMesh = new Mesh ();
				MeshGeneration.SphereMesh.GenerateMesh (sphereMesh);
				CylinderMesh.GenerateMesh (cylinderMesh);

				// Create materials
				materials = new Material[shaderPaths.Length];
				for (int i = 0; i < materials.Length; i++) {
					materials[i] = new Material (Shader.Find (shaderPaths[i]));
				}
			}

			// New frame index, so clear out last frame's draw list
			if (lastFrameInputReceived != Time.frameCount) {
				lastFrameInputReceived = Time.frameCount;

				// Store all unique meshes in inactive queue to be recycled
				var usedMeshes = new HashSet<Mesh> ();
				// Don't recycle cached meshes
				usedMeshes.Add (sphereMesh);
				usedMeshes.Add (cylinderMesh);

				for (int i = 0; i < drawList.Count; i++) {
					if (!usedMeshes.Contains (drawList[i].mesh)) {
						usedMeshes.Add (drawList[i].mesh);
						inactiveMeshes.Enqueue (drawList[i].mesh);
					}
				}

				// Clear old draw list
				drawList.Clear ();
			}
		}

		public static void CreateVisualElement (Mesh mesh, Vector3 position, Quaternion rotation, Vector3 scale) {
			Init ();
			drawList.Add (new VisualElement (mesh, position, rotation, scale, Visualizer.activeColour, Visualizer.activeStyle));
		}

		public static void AddVisualElement (VisualElement element) {
			Init ();
			drawList.Add (element);
		}

		// Draw all items in the drawList on each game/scene camera
		static void Draw (Camera camera) {
			if (camera && Time.frameCount == lastFrameInputReceived) {
				for (int i = 0; i < drawList.Count; i++) {
					VisualElement drawData = drawList[i];
					Matrix4x4 matrix = Matrix4x4.TRS (drawData.position, drawData.rotation, drawData.scale);

					materialProperties.SetColor ("_Color", drawData.colour);
					Material activeMaterial = materials[(int) drawData.style];
					Graphics.DrawMesh (drawData.mesh, matrix, activeMaterial, 0, camera, 0, materialProperties);
				}
			}
		}

		public static Mesh CreateOrRecycleMesh () {
			Mesh mesh = null;
			if (inactiveMeshes.Count > 0) {
				mesh = inactiveMeshes.Dequeue ();
			}

			if (mesh) {
				mesh.Clear ();
			} else {
				mesh = new Mesh ();
			}

			return mesh;
		}
	}
}