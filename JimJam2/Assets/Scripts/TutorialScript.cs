using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour {

	public bool activateCombatUI;
	public string tutorialString;

	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {
			if(activateCombatUI) {
				UserInterfaceManager.instance.ShowCombat();
			} else {
				UserInterfaceManager.instance.ShowBottomText(tutorialString);
			}
		}
	}
}
