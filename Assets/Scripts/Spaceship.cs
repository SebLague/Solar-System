using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody))]
public class Spaceship : GravityObject {
    public float mass;
    public float acceleration;
    public float rotSpeed;
    public float rollSpeed;
    public float rotSmoothSpeed;
    public bool lockCursor;
    Rigidbody rb;
    Quaternion targetRot;
    Quaternion smoothedRot;
    Vector3 input;

    public LayerMask groundedMask;
    int numCollisionTouches;

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
        HandleMovement ();
    }

    void HandleMovement () {
        DebugHelper.HandleEditorInput (lockCursor);

        int verticalThrust = 0;
        if (Input.GetKey (KeyCode.Space)) {
            if (Input.GetKey (KeyCode.LeftShift)) {
                verticalThrust = -1;
            } else {
                verticalThrust = 1;
            }
        }

        input = new Vector3 (Input.GetAxisRaw ("Horizontal"), verticalThrust, Input.GetAxisRaw ("Vertical"));

        float yawInput = Input.GetAxisRaw ("Mouse X") * rotSpeed;
        float pitchInput = Input.GetAxisRaw ("Mouse Y") * rotSpeed;
        float rollInput = (Input.GetKey (KeyCode.Q) ? -1 : Input.GetKey (KeyCode.E) ? 1 : 0) * rollSpeed * Time.deltaTime;

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
        Debug.Log("Grounded: " + (numCollisionTouches > 0));
    }

    void FixedUpdate () {

        var bodies = FindObjectsOfType<CelestialBody> ();
        // gravity
        foreach (var body in bodies) {
            var offsetToBody = (Vector3) body.transform.position - rb.position;
            var sqrDst = offsetToBody.sqrMagnitude;
            var dirToBody = offsetToBody / Mathf.Sqrt (sqrDst);
            var acceleration = Universe.gravitationalConstant * body.mass / sqrDst;
            rb.AddForce (dirToBody * acceleration, ForceMode.Acceleration);
        }

        // thrust
        Vector3 rocketAcceleration = transform.TransformVector (input) * acceleration;
        rb.AddForce (rocketAcceleration, ForceMode.Acceleration);

        if (numCollisionTouches == 0) {
            rb.MoveRotation (smoothedRot);
        }

    }

    void OnCollisionEnter (Collision other) {
        if (groundedMask == (groundedMask | (1 << other.gameObject.layer))) {
            //groundCols.Add (other);
            numCollisionTouches++;
        }
    }

    void OnCollisionExit (Collision other) {
        if (groundedMask == (groundedMask | (1 << other.gameObject.layer))) {
            numCollisionTouches--;
        }
    }

    void InitRigidbody () {
        if (rb == null) {
            rb = GetComponent<Rigidbody> ();
        }
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.mass = mass;
        rb.centerOfMass = Vector3.zero;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
    }

    void OnValidate () {
        InitRigidbody ();
    }
}