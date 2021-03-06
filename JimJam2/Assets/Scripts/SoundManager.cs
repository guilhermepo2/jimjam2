﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager instance;
	public AudioSource musicSource;
	public AudioSource sfxSource;

	void Awake() {
		if(instance == null) {
			instance = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}

	public void ChangeMusic(AudioClip music) {
		musicSource.Stop();
		musicSource.clip = music;
		musicSource.Play();
	}

	public void PlaySfx(AudioClip clip) {
		sfxSource.PlayOneShot(clip);
	}
}
