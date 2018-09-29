using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwooshScript : MonoBehaviour {

	// oh boy, there's some bad code here.
	public int animationFrames = 4;
	private Animator m_animator;

	void Start() {
		m_animator = GetComponent<Animator>();
	}

	void Update() {
		if(m_animator.GetCurrentAnimatorStateInfo(0).IsName("OVER")) {
			Destroy(gameObject);
		}
	}

	public void FlipSwoosh(float sign) {
		transform.localScale = new Vector3(sign * transform.localScale.x, transform.localScale.y, transform.localScale.z);
	}

	
}
