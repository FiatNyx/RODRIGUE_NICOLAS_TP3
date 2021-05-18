using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class MusicManager : MonoBehaviour
{
	public static MusicManager singleton;
	AudioSource musiqueSource;

	public AudioClip[] listeMusique;

	private float timerLerp = 1f;
	private float target;
	private float targetBas = 0.9f;
	private float targetHaut = 1f;
	float vitesseLerp = 0.4f;
	public PostProcessVolume activeVolume;
	Vignette vignette;


	/// <summary>
	/// Initialise le singleton s'il n'y en a pas déjà un.
	/// </summary>
	private void Awake()
	{
		if (singleton != null)
		{
			Debug.LogError("Détection de multiples instances du MusicManager.");
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
		
		//Post-processing volume pour la vignette
		if (activeVolume != null)
		{
			
			activeVolume.profile.TryGetSettings(out vignette);
			
		}
	}

	/// <summary>
	/// Ralentit la musique
	/// </summary>
	public void slowDownMusic()
	{
		target = targetBas;
		
	}


	/// <summary>
	/// Remet la musique à une vitesse normale
	/// </summary>
	public void normalMusicSpeed()
	{
		target = targetHaut;
		
	}

	/// <summary>
	/// Pause ou reprend la musique
	/// </summary>
	public void ToggleMusic()
	{
		if (musiqueSource.isPlaying)
		{
			musiqueSource.Pause();
		}
		else
		{
			musiqueSource.UnPause();
		}
	}

	/// <summary>
	/// Fait une transition entre la musique ralentie et la musique normale
	/// </summary>
    private void Update()
    {
		if (target == targetBas && timerLerp > targetBas)
		{
			timerLerp -= Time.deltaTime * vitesseLerp;
		}
		else if (target == targetHaut && timerLerp < targetHaut)
        {
			timerLerp += Time.deltaTime * vitesseLerp;
		}

		if(timerLerp > targetHaut)
        {
			timerLerp = targetHaut;
        }

		if(timerLerp < targetBas)
        {
			timerLerp = targetBas;
        }

		musiqueSource.pitch = timerLerp;

		//La vignette pour le ralentissement
		if (vignette != null) { vignette.intensity.value = 1f - timerLerp * 0.7f; }
	}
}
