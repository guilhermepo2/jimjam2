using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public static DialogueManager instance;
	[Header("UI Elements")]
	public GameObject dialoguePanel;
	public Text dialogueText;
	public GameObject pressSomethingToContinueText;

	private Queue<string> m_sentences;
	private Queue<Dialogue> m_dialogues;
	private string m_currentSentenceBeingTyped;
	private bool m_isTypingSentence;

	void Awake() {
		if (instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}

		m_sentences = new Queue<string>();
		m_dialogues = new Queue<Dialogue>();
		dialoguePanel.SetActive(false);
		pressSomethingToContinueText.SetActive(false);
	}

	void Update() {
		if(Input.GetButtonDown("Submit") || Input.GetKeyDown(KeyCode.Return)) {
			DisplayNextSentence();
		}
	}

	public void StartDialogue(Dialogue dialogue) {
		dialoguePanel.SetActive(true);
		m_sentences.Clear();

		foreach(string sentence in dialogue.sentences) {
			m_sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	public void StartDialogue(Dialogue[] dialogues) {
		m_dialogues.Clear();
		foreach(Dialogue dialogue in dialogues) {
			m_dialogues.Enqueue(dialogue);
		}

		StartDialogue(m_dialogues.Dequeue());
	}

	public void DisplayNextSentence() {
		if(m_isTypingSentence) {
			StopAllCoroutines();
			dialogueText.text = m_currentSentenceBeingTyped;
			pressSomethingToContinueText.SetActive(true);
			m_isTypingSentence = false;
			return;
		}

		if(m_sentences.Count == 0) {
			EndDialog();
			return;
		}

		string sentence = m_sentences.Dequeue();
		StopAllCoroutines();
		StartCoroutine(TypeSentence(sentence));
	}

	IEnumerator TypeSentence(string sentence) {
		m_isTypingSentence = true;
		pressSomethingToContinueText.SetActive(false);
		m_currentSentenceBeingTyped = sentence;
		dialogueText.text = "";

		foreach(char letter in sentence.ToCharArray()) {
			dialogueText.text += letter;
			yield return null;
		}

		pressSomethingToContinueText.SetActive(true);
		m_isTypingSentence = false;
	}

	void EndDialog() {
		if(m_dialogues.Count > 0) {
			StartDialogue(m_dialogues.Dequeue());
		} else {
			dialoguePanel.SetActive(false);
		}
	}
}

