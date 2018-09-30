using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivationScript : MonoBehaviour {

	public BossScript bossToActivate;

	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {
			Debug.Log("Boss Activated!");
			bossToActivate.Activate();
		}
	}
}
