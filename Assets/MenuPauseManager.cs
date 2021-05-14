using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPauseManager : MonoBehaviour
{
	public SoundManager soundManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
