using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour {

	private WarriorController playerReference;
	private Rigidbody2D m_rigidbody;
	private SpriteRenderer m_spriteRenderer;
	public AudioClip bossMusic;
	
	public float velocity = 5f;
	public float dashVelocity = 50f;
	public float dashLoadTime = 1f;

	private bool m_isActivated = false;
	private int m_health = 3;
	private bool m_canAct = false;
	private float m_originalGravity;
	private bool m_justGotHit = false;
	private bool m_isInvencible = false;


	void Start() {
		playerReference = FindObjectOfType<WarriorController>();
		m_rigidbody = GetComponent<Rigidbody2D>();
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_originalGravity = m_rigidbody.gravityScale;
	}

	void MoveTowardsPlayer() {
		Vector2 vectorToPlayer = playerReference.transform.position - transform.position;
		m_rigidbody.velocity = vectorToPlayer * velocity;
	}


	IEnumerator DashToPlayerRoutine() {
		float timeElapsed = 0f;
		m_rigidbody.gravityScale = 0f;
		while(timeElapsed < dashLoadTime) {
			Vector2 currentPosition = transform.position;
			currentPosition += (Vector2.up.normalized * 0.0075f);
			transform.position = currentPosition;

			timeElapsed += Time.deltaTime;
			yield return null;
		}

		Vector2 direction = playerReference.transform.position - transform.position;

		yield return new WaitForSeconds(dashLoadTime / 2f);

		m_rigidbody.velocity = direction * dashVelocity;
	}

	IEnumerator DashRoutine() {
		Debug.Log("Dash Routine");
		float yDestination = playerReference.transform.position.x;
		float timeElapsed = 0f;
		m_rigidbody.gravityScale = 0f;
		while(timeElapsed < dashLoadTime) {
			Vector2 currentPosition = transform.position;
			currentPosition += (Vector2.up.normalized * 0.005f);
			transform.position = currentPosition;

			timeElapsed += Time.deltaTime;
			yield return null;
		}
		m_rigidbody.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * dashVelocity, 0f);
	}

	void DashToPlayerDirection() {
		m_canAct = false;
		m_justGotHit = false;
		StopAllCoroutines();
		StartCoroutine(DashToPlayerRoutine());
	}
	
	void DashForward() {
		m_canAct = false;
		StopAllCoroutines();
		StartCoroutine(DashRoutine());
	}

	void Update() {
		if(!m_isActivated || !m_canAct) return;

		if(m_justGotHit) {
			DashToPlayerDirection();
		} else {
			DashForward();
		}
	}
	
	public void Activate() {
		m_isActivated = true;
		LookToThePlayer();
		StartCoroutine(ActionDelay(2.0f));
	}

	public void LookToThePlayer() {
		Vector2 vectorToPlayer = playerReference.transform.position - transform.position;
		transform.localScale = new Vector3(Mathf.Sign(vectorToPlayer.x), transform.localScale.y, transform.localScale.z);
		m_rigidbody.velocity = vectorToPlayer.normalized;
	}

	void OnCollisionEnter2D(Collision2D other) {
		if(!m_isActivated) return; 
		
		m_rigidbody.gravityScale = m_originalGravity;

		if(other.gameObject.tag != "Player") {
			m_rigidbody.velocity = Vector2.zero;
			LookToThePlayer();
			StartCoroutine(ActionDelay(2.0f));
		}
	}

	IEnumerator ActionDelay(float timeToWait) {
		yield return new WaitForSeconds(timeToWait / 2f);
		LookToThePlayer();
		yield return new WaitForSeconds(timeToWait / 2f);
		m_canAct = true;
	}

	private void Knockback(float sign) {
		m_canAct = false;

		StopAllCoroutines();
		m_rigidbody.gravityScale = m_originalGravity;

		m_rigidbody.velocity = new Vector2(sign * 10f, 0f);
		StartCoroutine(ActionDelay(1.0f));
	}

	private IEnumerator InvencibleRoutine() {
		float timeElapsed = 0f;
		while(timeElapsed < 1f) {
			timeElapsed += Time.deltaTime;
			m_spriteRenderer.enabled = !m_spriteRenderer.enabled;
			yield return null;
		}
		m_spriteRenderer.enabled = true;
		m_isInvencible = false;
	}

	public void PlayerAttacked(int damage, float sign) {
		if(!m_isActivated) {
			if(bossMusic) {
				SoundManager.instance.ChangeMusic(bossMusic);
			}
			Activate();
		} else {
			if(!m_isInvencible) {
				m_isInvencible = true;
				m_health-=damage;
			
				if(m_health <= 0) {
					UserInterfaceManager.instance.GameOver();
				}

				Knockback(sign);
				StartCoroutine(InvencibleRoutine());
				m_justGotHit = true;
			}
		}
	}
}
