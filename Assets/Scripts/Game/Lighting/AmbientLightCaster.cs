using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientLightCaster : MonoBehaviour {
    public float maxIntensity = 1;
    SunShadowCaster sunLight;
    Transform camT;
    Light ambientLight;

    void Start () {
        sunLight = FindObjectOfType<SunShadowCaster> ();
        ambientLight = GetComponent<Light> ();
        camT = Camera.main.transform;
        transform.rotation = CalculateAmbientLightRot ();
    }

    void LateUpdate () {

        transform.rotation = Quaternion.Slerp (transform.rotation, CalculateAmbientLightRot (), Time.deltaTime * .2f);
        float sunAlignment = Vector3.Dot (sunLight.transform.forward, transform.forward);
        float i = 1 - Mathf.Clamp01 (sunAlignment); // sun in same dir = 0, sun perpendicular = 1
        float intensityMultiplier = Mathf.Clamp01 ((i - 0.5f) * 2);
        ambientLight.intensity = maxIntensity * intensityMultiplier;
    }

    Quaternion CalculateAmbientLightRot () {
        CelestialBody[] bodies = NBodySimulation.Bodies;
        Vector3 nearestPlanetToCam = Vector3.zero;
        float nearestSqrDst = float.PositiveInfinity;

        for (int i = 0; i < bodies.Length; i++) {
            float sqrDst = (camT.position - bodies[i].transform.position).sqrMagnitude;
            if (sqrDst < nearestSqrDst) {
                nearestSqrDst = sqrDst;
                nearestPlanetToCam = bodies[i].transform.position;
            }
        }

        Vector3 targetDir = (nearestPlanetToCam - camT.position).normalized;
        Quaternion targetRot = Quaternion.LookRotation (targetDir);
        return targetRot;
    }

    void OnValidate () {
        GetComponent<Light> ().intensity = maxIntensity;
    }
}