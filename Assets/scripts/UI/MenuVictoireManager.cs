using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuVictoireManager : MonoBehaviour
{

	public void Recommencer()
	{
		Scene scene = SceneManager.GetActiveScene();
		SceneManager.LoadScene(scene.name);
	}

	public void Quitter()
	{
		SceneManager.LoadScene(0);
	}



}
