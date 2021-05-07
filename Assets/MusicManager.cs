using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
	public static MusicManager singleton;
	AudioSource musiqueSource;

	public AudioClip[] listeMusique;
	/// <summary>
	/// Initialise le singleton s'il n'y en a pas déjà un.
	/// </summary>
	private void Awake()
	{
		if (singleton != null)
		{
			Debug.LogError("Détection de multiples instances du GameManager.");
			return;
		}

		// Assignation du singleton
		singleton = this;
	}

	private void Start()
	{
		musiqueSource = GetComponent<AudioSource>();
		musiqueSource.clip = listeMusique[0];
		musiqueSource.Play();
	}

	public void slowDownMusic()
	{
		musiqueSource.pitch = 0.25f;
	}

	public void normalMusicSpeed()
	{
		musiqueSource.pitch = 1f;
	}
}
