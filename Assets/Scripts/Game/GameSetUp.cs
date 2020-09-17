using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetUp : MonoBehaviour {
	public enum StartCondition { InShip, OnBody }

	public StartCondition startCondition;
	public CelestialBody startBody;

	void Start () {
		Ship ship = FindObjectOfType<Ship> ();
		PlayerController player = FindObjectOfType<PlayerController> ();

		if (startCondition == StartCondition.InShip) {
			ship.PilotShip ();
			ship.flightControls.ForcePlayerInInteractionZone ();
		} else if (startCondition == StartCondition.OnBody) {
			if (startBody) {
				Vector3 pointAbovePlanet = startBody.transform.position + Vector3.right * startBody.radius * 1.1f;
				player.transform.position = pointAbovePlanet;
				player.SetVelocity (startBody.initialVelocity);
				ship.transform.position = pointAbovePlanet + Vector3.right * 20;
				ship.SetVelocity (startBody.initialVelocity);
				ship.ToggleHatch ();
			}
		}
	}
}