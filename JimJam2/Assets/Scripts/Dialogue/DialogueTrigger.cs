using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public Dialogue[] dialogues;

	void Start() {
		DialogueManager.instance.StartDialogue(dialogues);
	}
}
