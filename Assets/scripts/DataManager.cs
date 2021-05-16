using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DataManager : MonoBehaviour
{
	public static DataManager singleton;
	public string difficulte = "Normal"; //Facile, Normal, Difficile



	void Start()
    {
        
    }
	private void Awake()
	{
	
		if (singleton != null)
		{
			Debug.Log("Détection de multiples instances du GameManager.");
			Destroy(gameObject);
			return;
		}

		// Assignation du singleton
		singleton = this;
		DontDestroyOnLoad(this.gameObject);
	}
	// Update is called once per frame
	void Update()
    {
        
    }
}
