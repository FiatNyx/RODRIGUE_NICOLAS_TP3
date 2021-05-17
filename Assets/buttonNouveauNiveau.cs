using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonNouveauNiveau : MonoBehaviour
{
	public int lvlID = -1;
	public Text focusText;
	public Text toursText;
	public Text degatText;

	public Dropdown difficulte;


	private void Awake()
	{
		
		GetComponent<Button>().onClick.AddListener(btnJouer_OnClick);
	}

	void btnJouer_OnClick()
	{


		//Changer de scène
		
		SceneManager.LoadScene(lvlID);
	}

	public void changerLevelSelectionne(int id)
	{
		lvlID = id;
		float focus = PlayerPrefs.GetFloat("focus_lvl" + id.ToString() + DataManager.singleton.difficulte, 0f);
		int tours = PlayerPrefs.GetInt("tours_lvl" + id.ToString() + DataManager.singleton.difficulte, 0);
		int degats = PlayerPrefs.GetInt("degats_lvl" + id.ToString() + DataManager.singleton.difficulte, 0);

		focusText.text = focus.ToString() + "s";
		toursText.text = tours.ToString() + " tours";
		degatText.text = degats.ToString() + " dgt";
		GetComponent<Button>().interactable = true;
	}

	public void UpdateDifficulte()
	{
		switch (difficulte.value)
		{
			case 0:
				DataManager.singleton.difficulte = "Facile";
				break;
			case 1:
				DataManager.singleton.difficulte = "Normal";
				break;
			case 2:
				DataManager.singleton.difficulte = "Difficile";
				break;
			default:
				break;
		}

		if (lvlID != -1)
		{
			float focus = PlayerPrefs.GetFloat("focus_lvl" + lvlID.ToString() + DataManager.singleton.difficulte, 0f);
			int tours = PlayerPrefs.GetInt("tours_lvl" + lvlID.ToString() + DataManager.singleton.difficulte, 0);
			int degats = PlayerPrefs.GetInt("degats_lvl" + lvlID.ToString() + DataManager.singleton.difficulte, 0);

			focusText.text = focus.ToString() + "s";
			toursText.text = tours.ToString() + " tours";
			degatText.text = degats.ToString() + " dgt";
		}
		
	}
}
