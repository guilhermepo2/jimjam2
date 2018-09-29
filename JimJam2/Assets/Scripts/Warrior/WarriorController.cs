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

	// Private Variables for Internal Control
	private bool m_isAlive;
	private Rigidbody2D m_rigidbody;
	private BoxCollider2D m_feetCollider;
	private Animator m_animator;

	
	void Start () {
		m_rigidbody = GetComponent<Rigidbody2D>();
		m_feetCollider = GetComponent<BoxCollider2D>();
		m_animator = GetComponent<Animator>();
		m_isAlive = true;
		Time.timeScale = 1.0f;
	}
	
	void Update () {
		if(!m_isAlive) return;

		Run();
		Jump();	
		FlipSprite();
		AnimationLogic();
	}

	void Run() {
		float movement = m_rigidbody.velocity.x;
		movement += Input.GetAxisRaw("Horizontal");
		movement *= Mathf.Pow(1f - horizontalDamping, Time.deltaTime * 10f);

		movement = Mathf.Clamp(movement, -maxPlayerVelocity, maxPlayerVelocity);
		m_rigidbody.velocity = new Vector2(movement, m_rigidbody.velocity.y);
	}

	void Jump() {
		m_groundedRemember -= Time.fixedDeltaTime;
		m_jumpPressedRemember -= Time.fixedDeltaTime;

		if(m_feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) {
			m_groundedRemember = groundedRememberTime;
		}

		if(Input.GetButtonDown("Jump")) {
			m_jumpPressedRemember = jumpPressedRememberTime;
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
			// jump sound here
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


}
