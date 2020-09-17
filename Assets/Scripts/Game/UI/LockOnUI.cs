using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LockOnUI : MonoBehaviour {

	public int numSegments = 50;
	public Material mat;
	public float thickness;

	[Header ("Locked On")]
	public float lockedRadiusMultiplier = 1.2f;
	public float lockedAngle;
	public Color lockedColor;
	public Vector2 surfaceDstFadeOutRange = new Vector2 (250, 80);

	[Header ("Aimed")]
	public float aimedRadiusMutliplier = 1.3f;
	public float aimedAngle;
	public Color aimedColor;

	Camera playerCam;
	MaterialPropertyBlock materialProperties;
	Mesh lockedOnMesh;
	Mesh aimMesh;

	void Init () {
		if (materialProperties == null) {
			materialProperties = new MaterialPropertyBlock ();
		}
		if (lockedOnMesh == null) {
			lockedOnMesh = new Mesh ();
		}
		if (aimMesh == null) {
			aimMesh = new Mesh ();
		}

		if (playerCam == null) {
			playerCam = Camera.main;
		}
	}

	public void DrawLockOnUI (CelestialBody body, bool lockedOn) {
		Init ();

		Vector3 bodyCentre = body.transform.position;
		float pixelsPerUnit = (playerCam.WorldToScreenPoint (bodyCentre) - playerCam.WorldToScreenPoint (bodyCentre + playerCam.transform.up)).magnitude;
		float worldThickness = thickness / pixelsPerUnit;

		float innerRadius = body.radius * ((lockedOn) ? lockedRadiusMultiplier : aimedRadiusMutliplier);
		float outerRadius = innerRadius + worldThickness;

		int numIncrements = (int) Mathf.Max (5, numSegments);

		float angle = (lockedOn) ? lockedAngle : aimedAngle;
		float angleIncrement = (angle / (numIncrements - 1f)) * Mathf.Deg2Rad;

		var verts = new Vector3[numIncrements * 2];
		var norms = new Vector3[numIncrements * 2];
		var tris = new int[(numIncrements - 1) * 2 * 3];

		// Calculate verts and triangles
		for (int i = 0; i < numIncrements; i++) {
			float currAngle = angleIncrement * i;
			Vector3 dir = new Vector3 (Mathf.Cos (currAngle), Mathf.Sin (currAngle));
			verts[i * 2] = dir * innerRadius;
			verts[i * 2 + 1] = dir * outerRadius;

			if (i < numIncrements - 1) {
				tris[i * 6] = i * 2;
				tris[i * 6 + 1] = i * 2 + 1;
				tris[i * 6 + 2] = i * 2 + 2;

				tris[i * 6 + 3] = i * 2 + 2;
				tris[i * 6 + 4] = i * 2 + 1;
				tris[i * 6 + 5] = i * 2 + 3;
			}
		}

		Mesh mesh = (lockedOn) ? lockedOnMesh : aimMesh;

		mesh.vertices = verts;
		mesh.triangles = tris;
		mesh.RecalculateBounds ();

		// Draw mesh 4 times around planet
		float drawAngle = 45 - angle / 2;
		var dirToPlayer = (playerCam.transform.position - body.transform.position).normalized;
		var rot = Quaternion.AngleAxis (drawAngle, dirToPlayer) * Quaternion.LookRotation (dirToPlayer, playerCam.transform.up);
		var rot90 = Quaternion.AngleAxis (90, dirToPlayer);

		float dstToBodySurface = Mathf.Max (0, (playerCam.transform.position - body.transform.position).magnitude - body.radius);
		float alpha = Mathf.InverseLerp (surfaceDstFadeOutRange.y, surfaceDstFadeOutRange.x, dstToBodySurface);
		Color displayCol = (lockedOn) ? lockedColor : aimedColor;

		displayCol = new Color (displayCol.r, displayCol.g, displayCol.b, alpha);
		materialProperties.SetColor ("_Color", displayCol);

		for (int i = 0; i < 4; i++) {
			rot = rot90 * rot;
			Graphics.DrawMesh (mesh, body.transform.position, rot, mat, 0, null, 0, materialProperties, false, false, false);
			drawAngle += 90;
		}
	}

}