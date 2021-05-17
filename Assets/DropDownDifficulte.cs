using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropDownDifficulte : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		switch (DataManager.singleton.difficulte)
		{
			case "Facile":
				GetComponent<Dropdown>().value = 0;
				break;
			case "Normal":
				GetComponent<Dropdown>().value = 1;
				break;
			case "Difficile":
				GetComponent<Dropdown>().value = 2;
				break;
			default:
				break;
		}

		
    }

 
}
