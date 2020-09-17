using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent (typeof (Rigidbody))]
public class CelestialBody : GravityObject {

    public enum BodyType { Planet, Moon, Sun }
    public BodyType bodyType;
    public float radius;
    public float surfaceGravity;
    public Vector3 initialVelocity;
    public string bodyName = "Unnamed";
    Transform meshHolder;

    public Vector3 velocity { get; private set; }
    public float mass { get; private set; }
    Rigidbody rb;

    void Awake () {

        rb = GetComponent<Rigidbody> ();
        velocity = initialVelocity;
        RecalculateMass ();
    }

    public void UpdateVelocity (CelestialBody[] allBodies, float timeStep) {
        foreach (var otherBody in allBodies) {
            if (otherBody != this) {
                float sqrDst = (otherBody.rb.position - rb.position).sqrMagnitude;
                Vector3 forceDir = (otherBody.rb.position - rb.position).normalized;

                Vector3 acceleration = forceDir * Universe.gravitationalConstant * otherBody.mass / sqrDst;
                velocity += acceleration * timeStep;
            }
        }
    }

    public void UpdateVelocity (Vector3 acceleration, float timeStep) {
        velocity += acceleration * timeStep;
    }

    public void UpdatePosition (float timeStep) {
        rb.MovePosition (rb.position + velocity * timeStep);

    }

    void OnValidate () {
        RecalculateMass ();
        if (GetComponentInChildren<CelestialBodyGenerator> ()) {
            GetComponentInChildren<CelestialBodyGenerator> ().transform.localScale = Vector3.one * radius;
        }
        gameObject.name = bodyName;
    }

    public void RecalculateMass () {
        mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
        Rigidbody.mass = mass;
    }

    public Rigidbody Rigidbody {
        get {
            if (!rb) {
                rb = GetComponent<Rigidbody> ();
            }
            return rb;
        }
    }

    public Vector3 Position {
        get {
            return rb.position;
        }
    }

}