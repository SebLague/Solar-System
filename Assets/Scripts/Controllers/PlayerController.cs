using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : GravityObject {

    public LayerMask walkableMask;

    public float maxAcceleration;
    public Transform feet;
    public float walkSpeed = 8;
    public float runSpeed = 14;
    public float jumpForce = 20;
    public float vSmoothTime = 0.1f;
    public float airSmoothTime = 0.5f;
    public float stickToGroundForce = 8;

    public bool lockCursor;
    public float mass = 70;
    Rigidbody rb;
    Ship spaceship;

    public float mouseSensitivity = 10;
    public Vector2 pitchMinMax = new Vector2 (-40, 85);
    public float rotationSmoothTime = 0.1f;

    public float yaw;
    public float pitch;
    float smoothYaw;
    float smoothPitch;

    float yawSmoothV;
    float pitchSmoothV;

    public Vector3 targetVelocity;
    Vector3 cameraLocalPos;
    Vector3 smoothVelocity;
    Vector3 smoothVRef;

    CelestialBody referenceBody;

    Camera cam;
    bool readyToFlyShip;
    public Vector3 delta;

    void Awake () {
        cam = GetComponentInChildren<Camera> ();
        cameraLocalPos = cam.transform.localPosition;
        spaceship = FindObjectOfType<Ship> ();
        InitRigidbody ();

        if (lockCursor) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void InitRigidbody () {
        rb = GetComponent<Rigidbody> ();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.useGravity = false;
        rb.isKinematic = false;
        rb.mass = mass;
    }

    void Update () {
        HandleMovement ();
    }

    void HandleMovement () {
        DebugHelper.HandleEditorInput (lockCursor);
        // Look input
        yaw += Input.GetAxisRaw ("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxisRaw ("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp (pitch - Input.GetAxisRaw ("Mouse Y") * mouseSensitivity, pitchMinMax.x, pitchMinMax.y);
        smoothPitch = Mathf.SmoothDampAngle (smoothPitch, pitch, ref pitchSmoothV, rotationSmoothTime);
        float smoothYawOld = smoothYaw;
        smoothYaw = Mathf.SmoothDampAngle (smoothYaw, yaw, ref yawSmoothV, rotationSmoothTime);
        cam.transform.localEulerAngles = Vector3.right * smoothPitch;
        transform.Rotate (Vector3.up * Mathf.DeltaAngle (smoothYawOld, smoothYaw), Space.Self);

        // Movement
        bool isGrounded = IsGrounded ();
        Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
        float currentSpeed = Input.GetKey (KeyCode.LeftShift) ? runSpeed : walkSpeed;
        targetVelocity = transform.TransformDirection (input.normalized) * currentSpeed;
        smoothVelocity = Vector3.SmoothDamp (smoothVelocity, targetVelocity, ref smoothVRef, (isGrounded) ? vSmoothTime : airSmoothTime);

        if (isGrounded) {
            if (Input.GetKeyDown (KeyCode.Space)) {
                rb.AddForce (transform.up * jumpForce, ForceMode.VelocityChange);
                isGrounded = false;
            } else {
                // Apply small downward force to prevent player from bouncing when going down slopes
                rb.AddForce (-transform.up * stickToGroundForce, ForceMode.VelocityChange);
            }
        }
    }

    bool IsGrounded () {
        // Sphere must not overlay terrain at origin otherwise no collision will be detected
        // so rayRadius should not be larger than controller's capsule collider radius
        const float rayRadius = .3f;
        const float groundedRayDst = .2f;
        bool grounded = false;

        if (referenceBody) {
            var relativeVelocity = rb.velocity - referenceBody.velocity;
            // Don't cast ray down if player is jumping up from surface
            if (relativeVelocity.y <= jumpForce * .5f) {
                RaycastHit hit;
                Vector3 offsetToFeet = (feet.position - transform.position);
                Vector3 rayOrigin = rb.position + offsetToFeet + transform.up * rayRadius;
                Vector3 rayDir = -transform.up;

                grounded = Physics.SphereCast (rayOrigin, rayRadius, rayDir, out hit, groundedRayDst, walkableMask);
            }
        }

        return grounded;
    }

    void FixedUpdate () {
        CelestialBody[] bodies = NBodySimulation.Bodies;
        Vector3 strongestGravitionalPull = Vector3.zero;

        // Gravity
        foreach (CelestialBody body in bodies) {
            float sqrDst = (body.Position - rb.position).sqrMagnitude;
            Vector3 forceDir = (body.Position - rb.position).normalized;
            Vector3 acceleration = forceDir * Universe.gravitationalConstant * body.mass / sqrDst;
            rb.AddForce (acceleration, ForceMode.Acceleration);

            // Find body with strongest gravitational pull 
            if (acceleration.sqrMagnitude > strongestGravitionalPull.sqrMagnitude) {
                strongestGravitionalPull = acceleration;
                referenceBody = body;
            }
        }

        // Rotate to align with gravity up
        Vector3 gravityUp = -strongestGravitionalPull.normalized;
        rb.rotation = Quaternion.FromToRotation (transform.up, gravityUp) * rb.rotation;

        // Move
        rb.MovePosition (rb.position + smoothVelocity * Time.fixedDeltaTime);
    }

    public void SetVelocity (Vector3 velocity) {
        rb.velocity = velocity;
    }

    public void ExitFromSpaceship () {
        cam.transform.parent = transform;
        cam.transform.localPosition = cameraLocalPos;
        smoothYaw = 0;
        yaw = 0;
        smoothPitch = cam.transform.localEulerAngles.x;
        pitch = smoothPitch;
    }

    public Camera Camera {
        get {
            return cam;
        }
    }

    public Rigidbody Rigidbody {
        get {
            return rb;
        }
    }

}