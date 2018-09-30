﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceManager : MonoBehaviour {

	public static UserInterfaceManager instance;
	[Header("Powers Panels")]
	public GameObject couragePanel;
	public GameObject wisdomPanel;
	public Text bottomScreenText;

	private bool m_waitingForInput = false;
	private Queue<string> m_textQueue;
	
	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	void DisableAllPanels() {
		couragePanel.SetActive(false);
		wisdomPanel.SetActive(false);
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
		bottomScreenText.text = m_textQueue.Dequeue();
		StartCoroutine(EndSentenceOnBottom());
	}
	
	void Start () {
		m_textQueue = new Queue<string>();
		DisableAllPanels();
		m_textQueue.Enqueue("Use the ARROW KEYS to move");
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
			}
		}
	}

	public void ShowCourage() {
		couragePanel.SetActive(true);
		m_waitingForInput = true;
		m_textQueue.Enqueue("Jump while in midair to double jump");
		Time.timeScale = 0;
	}
}