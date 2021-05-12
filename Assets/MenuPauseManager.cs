using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPauseManager : MonoBehaviour
{
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
		GameManager.singleton.TogglePause();
	}

	public void Quitter()
	{
		SceneManager.LoadScene(0);
	}

	public void Recommencer()
	{
		SceneManager.LoadScene(0);
	}

}
