using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : GravityObject {

	public Transform hatch;
	public float hatchAngle;
	public Transform camViewPoint;
	public Transform pilotSeatPoint;
	public LayerMask groundedMask;
	public GameObject window;

	[Header ("Handling")]
	public float thrustStrength = 20;
	public float rotSpeed = 5;
	public float rollSpeed = 30;
	public float rotSmoothSpeed = 10;
	public bool lockCursor;

	[Header ("Interact")]
	public Interactable flightControls;

	Rigidbody rb;
	Quaternion targetRot;
	Quaternion smoothedRot;

	Vector3 thrusterInput;
	PlayerController pilot;
	bool shipIsPiloted;
	int numCollisionTouches;
	bool hatchOpen;

	KeyCode ascendKey = KeyCode.Space;
	KeyCode descendKey = KeyCode.LeftShift;
	KeyCode rollCounterKey = KeyCode.Q;
	KeyCode rollClockwiseKey = KeyCode.E;
	KeyCode forwardKey = KeyCode.W;
	KeyCode backwardKey = KeyCode.S;
	KeyCode leftKey = KeyCode.A;
	KeyCode rightKey = KeyCode.D;

	void Awake () {
		InitRigidbody ();
		targetRot = transform.rotation;
		smoothedRot = transform.rotation;

		if (lockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}

	void Update () {
		if (shipIsPiloted) {
			HandleMovement ();
		}

		// Animate hatch
		float hatchTargetAngle = (hatchOpen) ? hatchAngle : 0;
		hatch.localEulerAngles = Vector3.right * Mathf.LerpAngle (hatch.localEulerAngles.x, hatchTargetAngle, Time.deltaTime);

		HandleCheats ();
	}

	void HandleMovement () {
		DebugHelper.HandleEditorInput (lockCursor);
		// Thruster input
		int thrustInputX = GetInputAxis (leftKey, rightKey);
		int thrustInputY = GetInputAxis (descendKey, ascendKey);
		int thrustInputZ = GetInputAxis (backwardKey, forwardKey);
		thrusterInput = new Vector3 (thrustInputX, thrustInputY, thrustInputZ);

		// Rotation input
		float yawInput = Input.GetAxisRaw ("Mouse X") * rotSpeed;
		float pitchInput = Input.GetAxisRaw ("Mouse Y") * rotSpeed;
		float rollInput = GetInputAxis (rollCounterKey, rollClockwiseKey) * rollSpeed * Time.deltaTime;

		// Calculate rotation
		if (numCollisionTouches == 0) {
			var yaw = Quaternion.AngleAxis (yawInput, transform.up);
			var pitch = Quaternion.AngleAxis (-pitchInput, transform.right);
			var roll = Quaternion.AngleAxis (-rollInput, transform.forward);

			targetRot = yaw * pitch * roll * targetRot;
			smoothedRot = Quaternion.Slerp (transform.rotation, targetRot, Time.deltaTime * rotSmoothSpeed);
		} else {
			targetRot = transform.rotation;
			smoothedRot = transform.rotation;
		}
	}

	void FixedUpdate () {
		// Gravity
		Vector3 gravity = NBodySimulation.CalculateAcceleration (rb.position);
		rb.AddForce (gravity, ForceMode.Acceleration);

		// Thrusters
		Vector3 thrustDir = transform.TransformVector (thrusterInput);
		rb.AddForce (thrustDir * thrustStrength, ForceMode.Acceleration);

		if (numCollisionTouches == 0) {
			rb.MoveRotation (smoothedRot);
		}
	}

	void TeleportToBody (CelestialBody body) {
		rb.velocity = body.velocity;
		rb.MovePosition (body.transform.position + (transform.position - body.transform.position).normalized * body.radius * 2);
	}

	int GetInputAxis (KeyCode negativeAxis, KeyCode positiveAxis) {
		int axis = 0;
		if (Input.GetKey (positiveAxis)) {
			axis++;
		}
		if (Input.GetKey (negativeAxis)) {
			axis--;
		}
		return axis;
	}

	void HandleCheats () {
		if (Universe.cheatsEnabled) {
			if (Input.GetKeyDown (KeyCode.Return) && IsPiloted) {
				var shipHud = FindObjectOfType<ShipHUD> ();
				if (shipHud.LockedBody) {
					TeleportToBody (shipHud.LockedBody);
				}
			}
		}
	}

	void InitRigidbody () {
		rb = GetComponent<Rigidbody> ();
		rb.interpolation = RigidbodyInterpolation.Interpolate;
		rb.useGravity = false;
		rb.isKinematic = false;
		rb.centerOfMass = Vector3.zero;
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
	}

	public void ToggleHatch () {
		hatchOpen = !hatchOpen;
	}

	public void TogglePiloting () {
		if (shipIsPiloted) {
			StopPilotingShip ();
		} else {
			PilotShip ();
		}
	}

	public void PilotShip () {
		pilot = FindObjectOfType<PlayerController> ();
		shipIsPiloted = true;
		pilot.Camera.transform.parent = camViewPoint;
		pilot.Camera.transform.localPosition = Vector3.zero;
		pilot.Camera.transform.localRotation = Quaternion.identity;
		pilot.gameObject.SetActive (false);
		hatchOpen = false;
		window.SetActive (false);

	}

	void StopPilotingShip () {
		shipIsPiloted = false;
		pilot.transform.position = pilotSeatPoint.position;
		pilot.transform.rotation = pilotSeatPoint.rotation;
		pilot.Rigidbody.velocity = rb.velocity;
		pilot.gameObject.SetActive (true);
		window.SetActive (true);
		pilot.ExitFromSpaceship ();
	}

	void OnCollisionEnter (Collision other) {
		if (groundedMask == (groundedMask | (1 << other.gameObject.layer))) {
			numCollisionTouches++;
		}
	}

	void OnCollisionExit (Collision other) {
		if (groundedMask == (groundedMask | (1 << other.gameObject.layer))) {
			numCollisionTouches--;
		}
	}

	public void SetVelocity (Vector3 velocity) {
		rb.velocity = velocity;
	}

	public bool ShowHUD {
		get {
			return shipIsPiloted;
		}
	}
	public bool HatchOpen {
		get {
			return hatchOpen;
		}
	}

	public bool IsPiloted {
		get {
			return shipIsPiloted;
		}
	}

	public Rigidbody Rigidbody {
		get {
			return rb;
		}
	}

}