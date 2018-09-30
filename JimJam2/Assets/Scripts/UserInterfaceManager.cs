using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour {

	public static UserInterfaceManager instance;
	public AudioClip calmMusic;
	
	[Header("Player Health")]
	public GameObject lifePanel;
	public Image[] hearts;
	public Sprite fullHeart;
	public Sprite halfHeart;
	public Sprite emptyHeart;


	[Header("Powers Panels")]
	public GameObject couragePanel;
	public GameObject wisdomPanel;
	public GameObject powerPanel;
	public Text bottomScreenText;

	private bool m_waitingForInput = false;
	private Queue<string> m_textQueue;
	private bool m_gameOver = false;
	
	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}

		lifePanel.SetActive(false);
	}

	void DisableAllPanels() {
		couragePanel.SetActive(false);
		wisdomPanel.SetActive(false);
		powerPanel.SetActive(false);
	}

	IEnumerator EndSentenceOnBottom() {
		yield return new WaitForSeconds(5.0f);

		if(m_textQueue.Count > 0) {
			ShowBottomScreenText();
		} else {
			bottomScreenText.text = "";
		}
	}

	void ShowBottomScreenText() {
		if(m_textQueue.Count > 0) {
			bottomScreenText.text = m_textQueue.Dequeue();
			StartCoroutine(EndSentenceOnBottom());
		}
	}
	
	void Start () {
		m_textQueue = new Queue<string>();
		DisableAllPanels();
		m_textQueue.Enqueue("Use A and D or the ARROW KEYS to move");
		m_textQueue.Enqueue("Use SPACE to jump");
		ShowBottomScreenText();
	}

	
	void Update () {
		if(m_waitingForInput) {
			if(Input.GetButtonDown("Submit")) {
				m_waitingForInput = false;
				Time.timeScale = 1f;
				DisableAllPanels();

				// show next bottom text
				ShowBottomScreenText();

				if(m_gameOver) {
					SoundManager.instance.ChangeMusic(calmMusic);
					GameManagement.instance.LoadNextLevel();
				} 
			}
		}
	}

	public void ShowCourage() {
		couragePanel.SetActive(true);
		m_waitingForInput = true;
		m_textQueue.Enqueue("Jump while in midair to double jump");
		Time.timeScale = 0;
	}

	public void ShowWisdom() {
		wisdomPanel.SetActive(true);
		m_waitingForInput = true;
		m_textQueue.Enqueue("Press I to dash while on air");
		m_textQueue.Enqueue("The dash follow the keys you are pressing");
		Time.timeScale = 0;
	}

	public void ShowBottomText(string text) {
		if(m_textQueue.Count > 0) {
			m_textQueue.Enqueue(text);
			ShowBottomScreenText();
		}
	}

	public void ShowCombat() {
		lifePanel.SetActive(true);
		m_textQueue.Enqueue("Use O to attack");
		ShowBottomScreenText();
	}

	public void GameOver() {
		Debug.Log("Game Over!");
		Time.timeScale = 0.15f;
		powerPanel.SetActive(true);
		m_waitingForInput = true;
		m_gameOver = true;
		
		// Imply That The Cycle Begins Again

	}

	public void RenderHealth(int currentLife, int maxLife) {
		int heartsToDisplay = (int)Mathf.Ceil(maxLife / 2);
		int health = currentLife;

		for(int i = 0; i < heartsToDisplay; i++) {
			if(health >= 2) {
				hearts[i].enabled = true;
				hearts[i].sprite = fullHeart;
				health -= 2;
			} else if(health == 1) {
				hearts[i].enabled = true;
				hearts[i].sprite = halfHeart;
				health--;
			} else {
				hearts[i].enabled = true;
				hearts[i].sprite = emptyHeart;
				health = 0;
			}
		}
	}
}
