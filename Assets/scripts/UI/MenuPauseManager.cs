using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPauseManager : MonoBehaviour
{
	public SoundManager soundManager;

	public void FermerMenu()
	{
		soundManager.sauvegarderSon();
		GameManager.singleton.TogglePause();
	}

	public void Quitter()
	{
		soundManager.sauvegarderSon();
		SceneManager.LoadScene(0);
	}

	public void Recommencer()
	{
		soundManager.sauvegarderSon();
		SceneManager.LoadScene(0);
	}

}
