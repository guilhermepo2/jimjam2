using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorController : MonoBehaviour {

	[Header("Audio Handling")]
	public AudioClip jumpClip;
	public AudioClip hitClip;
	public AudioClip deathHitClip;

	[Header("Movement Handling")]
	public float maxPlayerVelocity = 5f;
	public float horizontalDamping = 0.25f;


	[Header("Jumping Handling")]
	public float jumpForce = 20f;
	public float jumpPressedRememberTime = 0.15f;
	public float groundedRememberTime = 0.15f;
	public float cutJumpHeight = 0.35f;
	private float m_jumpPressedRemember;
	private float m_groundedRemember;

	
	[Header("Attack Handling")]
	public GameObject swooshObject;
	public Vector2 attackKnockback = new Vector2(-5f, 5f);
	public float attackDelay = 0.25f;
	private float m_attackDelayElapsed;

	[Header("Damage Handling")]
	public Vector2 damageKnockback = new Vector2(10f, 2f);
	public float knockbackTime = .1f;
	public float invencibilityTime = 1f;
	private bool m_isInvincible = false;
	private Vector2 m_damageKnockBack = Vector2.zero;
	
	// Power Up Handlers
	[Header("Power Up: Double Jump Handling")]
	public bool hasDoubleJump;
	public float secondJumpForce = 15f;
	private bool m_canDoubleJump = false;

	[Header("Power Up: Air Dash Handling")]
	public bool hasAirDash;
	public Vector2 airDashForce = new Vector2(5f, 1f);


	// Private Variables for Internal Control
	private bool m_isAlive;
	private Rigidbody2D m_rigidbody;
	private BoxCollider2D m_feetCollider;
	private Animator m_animator;
	private SpriteRenderer m_spriteRenderer;

	
	void Start () {
		m_rigidbody = GetComponent<Rigidbody2D>();
		m_feetCollider = GetComponent<BoxCollider2D>();
		m_animator = GetComponent<Animator>();
		m_spriteRenderer = GetComponent<SpriteRenderer>();
		m_isAlive = true;
		Time.timeScale = 1.0f;
	}
	
	void Update () {
		if(!m_isAlive) return;

		Run();
		if(hasDoubleJump) DoubleJump();
		Jump();	
		ProcessAttack();
		FlipSprite();
		AnimationLogic();
	}

	void Run() {
		float movement = m_rigidbody.velocity.x;
		movement += Input.GetAxisRaw("Horizontal");
		movement *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);

		movement = Mathf.Clamp(movement, -maxPlayerVelocity, maxPlayerVelocity);
		m_rigidbody.velocity = new Vector2(movement, m_rigidbody.velocity.y) + m_damageKnockBack;
	}

	void Jump() {
		m_groundedRemember -= Time.fixedDeltaTime;
		m_jumpPressedRemember -= Time.fixedDeltaTime;

		if(m_feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
			m_groundedRemember = groundedRememberTime;
		}

		if(Input.GetButtonDown("Jump")) {
			m_jumpPressedRemember = jumpPressedRememberTime;
			m_canDoubleJump = false;
		}

		if(Input.GetButtonUp("Jump")) {
			if(m_rigidbody.velocity.y > 0) {
				m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, m_rigidbody.velocity.y * cutJumpHeight);
			}
		}

		if(m_jumpPressedRemember > 0 && m_groundedRemember > 0) {
			m_jumpPressedRemember = 0;
			m_groundedRemember = 0;

			m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, jumpForce);
			m_canDoubleJump = true;
			// jump sound here
		}
	}

	void DoubleJump() {
		if(m_canDoubleJump && Input.GetButtonDown("Jump")) {
			m_canDoubleJump = false;
			m_rigidbody.velocity = new Vector2(m_rigidbody.velocity.x, jumpForce);
			// jump sound here
		}
	}

	void ProcessAttack() {
		m_attackDelayElapsed -= Time.deltaTime;
		if(Input.GetKeyDown(KeyCode.O) && m_attackDelayElapsed <= 0) {
			m_attackDelayElapsed = attackDelay;

			Instantiate(swooshObject, this.transform.position + ((Vector3.right * Mathf.Sign(transform.localScale.x)) / 1.5f), Quaternion.identity).GetComponent<SwooshScript>().FlipSwoosh(Mathf.Sign(transform.localScale.x));

			m_rigidbody.velocity += new Vector2(attackKnockback.x * Mathf.Sign(transform.localScale.x), attackKnockback.y);
		}
	}

	void FlipSprite() {
		if(Mathf.Abs(m_rigidbody.velocity.x) > 0) {
			transform.localScale = new Vector3(Mathf.Sign(m_rigidbody.velocity.x), transform.localScale.y, transform.localScale.z);
		}
	}

	void AnimationLogic() {
		if(m_rigidbody.velocity.y >= 0.1f) {
			m_animator.Play("Jumping");
		} else if(m_rigidbody.velocity.y <= -0.1f) {
			m_animator.Play("Falling");
		} else if(Mathf.Abs(m_rigidbody.velocity.x) > 0.1f) {
			m_animator.Play("Running");
		} else {
			m_animator.Play("Idle");
		}
	}

	// ==========================================================================================
	// ==========================================================================================
	// 	COLLISION EVENTS
	// ==========================================================================================
	// ==========================================================================================
	private IEnumerator EndKnockback() {
		yield return new WaitForSeconds(knockbackTime);
		m_damageKnockBack = Vector2.zero;
	}

	private IEnumerator EndInvincibility() {
		float timeElapsed = 0;
		while(timeElapsed < invencibilityTime) {
			m_spriteRenderer.enabled = !m_spriteRenderer.enabled;
			timeElapsed += Time.deltaTime;
			yield return null;
		}
		m_spriteRenderer.enabled = true;
		m_isInvincible = false;
	}
	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.tag == "Enemy" && !m_isInvincible) {
			// Apply Knockback
			m_damageKnockBack = new Vector2(Mathf.Sign(other.gameObject.transform.localScale.x) * damageKnockback.x, damageKnockback.y);
			m_isInvincible = true;
			StartCoroutine(EndKnockback());
			StartCoroutine(EndInvincibility());
		}
	}

}
