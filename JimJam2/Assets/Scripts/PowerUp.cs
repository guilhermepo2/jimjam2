using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

	public bool isCourage;
	public bool isWisdom;
	private SpriteRenderer m_spriteRenderer;

	void Start() {
		m_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {
		this.transform.position = new Vector2(transform.position.x, transform.position.y + (Mathf.Sin(Time.timeSinceLevelLoad) * 0.01f));
	}

	private IEnumerator FadeOutRoutine() {
		float alpha = m_spriteRenderer.color.a;
		for(int i = 0; i < 10; i++) {
			alpha -= 0.1f;
			m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, alpha);
			yield return null;
		}
		Destroy(gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if(other.tag == "Player") {

			if(isCourage) {
				other.GetComponent<WarriorController>().hasDoubleJump = true;
				// show courage panel
				UserInterfaceManager.instance.ShowCourage();
			}

			StartCoroutine(FadeOutRoutine());
		}
	}
}
