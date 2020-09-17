using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    public string interactMessage = "Press F to interact";
    protected bool playerInInteractionZone;
    public UnityEngine.Events.UnityEvent interactEvent;

    protected virtual void Interact () {
        if (interactEvent != null) {
            interactEvent.Invoke ();
        }
    }

    protected virtual void Update () {
        if (playerInInteractionZone && Input.GetKeyDown (KeyCode.F)) {
            GameUI.CancelInteractionDisplay ();
            Interact ();
        }
    }

    protected virtual void OnTriggerEnter (Collider c) {
        if (c.tag == "Player") {
            playerInInteractionZone = true;
            ShowInteractMessage ();
        }
    }

    protected virtual void OnTriggerExit (Collider c) {
        if (c.tag == "Player") {
            GameUI.CancelInteractionDisplay ();
            playerInInteractionZone = false;
        }
    }
    protected virtual void ShowInteractMessage () {
        GameUI.DisplayInteractionInfo (interactMessage);
    }

    public void ForcePlayerInInteractionZone () {
        playerInInteractionZone = true;
    }

}