using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour {

	public int health = 2;
	public float enemyVelocity = 3f;
	public GameObject wallCheck;
	public Vector2 knockbackForce = new Vector2(10f, 2f);
	public float knockbackTime = 0.2f;
	public float invencibilityTime = 1f;


	private Rigidbody2D m_rigidbody;
	private Animator m_animator;
	private SpriteRenderer m_spriteRenderer;
	private Vector2 m_damageKnockBack = Vector2.zero;
	private bool m_isInvincible = false;

	void Start() {
		m_rigidbody = GetComponent<Rigidbody2D>();
		m_animator = GetComponent<Animator>();
		m_spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void Update() {

		// Velocity
		m_rigidbody.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * enemyVelocity, m_rigidbody.velocity.y) + m_damageKnockBack;

		// Animation and Flip Sprite
		if(Mathf.Abs(m_rigidbody.velocity.x) > 0) {
			m_animator.Play("Running");
		} else {
			m_animator.Play("Idle");
		}

		// Wall Handling
		Collider2D collision = Physics2D.OverlapCircle(wallCheck.transform.position, 0.1f);
		if(collision) {
			if(collision.tag != "Player") {
				transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
			}
		}
		
	}

	// ==========================================================================================
	// ==========================================================================================
	// 	DAMAGE EVENTS
	// ==========================================================================================
	// ==========================================================================================

	private IEnumerator EndKnockback() {
		yield return new WaitForSeconds(knockbackTime);
		m_damageKnockBack = Vector2.zero;
	}

	private IEnumerator EndInvencibility() {
		float timeElapsed = 0;
		while(timeElapsed < invencibilityTime) {
			m_spriteRenderer.enabled = !m_spriteRenderer.enabled;
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		m_spriteRenderer.enabled = true;
		m_isInvincible = false;
	}

	public void TakeDamage(int damage, float knockbackSign) {
		if(m_isInvincible) return;

		m_damageKnockBack = new Vector2(knockbackSign * knockbackForce.x, knockbackForce.y);
		StartCoroutine(EndKnockback());

		m_isInvincible = true;
		StartCoroutine(EndInvencibility());

		health -= damage;
		if(health <= 0) {
			Destroy(gameObject);
		}
	}
}
