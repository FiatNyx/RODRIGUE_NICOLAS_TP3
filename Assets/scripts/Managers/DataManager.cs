using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DataManager : MonoBehaviour
{
	public static DataManager singleton;
	public string difficulte = "Normal"; //Facile, Normal, Difficile


	
	/// <summary>
	/// Détection multiple singleton + capture d'inputs en webgl
	/// </summary>
	private void Awake()
	{
		#if !UNITY_EDITOR && UNITY_WEBGL
				WebGLInput.captureAllKeyboardInput = true;
		#endif
		if (singleton != null)
		{
			Debug.Log("Détection de multiples instances du DataManager.");
			Destroy(gameObject);
			return;
		}
		

		// Assignation du singleton
		singleton = this;
		DontDestroyOnLoad(this.gameObject);
	}

}
