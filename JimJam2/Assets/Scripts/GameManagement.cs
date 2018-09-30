using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagement : MonoBehaviour {

	public static GameManagement instance;

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	public void ReloadScene() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadNextLevel() {
		int t_sceneCount = SceneManager.sceneCountInBuildSettings;
		int t_sceneToLoad = (SceneManager.GetActiveScene().buildIndex + 1) % t_sceneCount;
		SceneManager.LoadScene(t_sceneToLoad);
	}
}
