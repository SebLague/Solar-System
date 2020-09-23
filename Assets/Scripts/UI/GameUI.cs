using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour {

    public TMPro.TMP_Text interactionInfo;
    float interactionInfoDisplayTimeRemaining;
    static GameUI instance;

    void Update () {
        if (interactionInfo) {
            interactionInfoDisplayTimeRemaining -= Time.deltaTime;
            interactionInfo.enabled = (interactionInfoDisplayTimeRemaining > 0);
        }
    }

    public static void DisplayInteractionInfo (string info) {
        if (Instance) {
            Instance.interactionInfo.text = info;
            Instance.interactionInfoDisplayTimeRemaining = 3;
        } else {
            Debug.Log ($"{info} (no UI instance found)");
        }
    }

    public static void CancelInteractionDisplay () {
        if (Instance) {
            Instance.interactionInfoDisplayTimeRemaining = 0;
        }
    }

    static GameUI Instance {
        get {
            if (instance == null) {
                instance = FindObjectOfType<GameUI> ();
            }
            return instance;
        }
    }
}