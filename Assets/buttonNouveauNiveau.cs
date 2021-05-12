using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonNouveauNiveau : MonoBehaviour
{
	public int lvlID;
	public Text focusText;
	public Text toursText;
	public Text degatText;

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
		float focus = PlayerPrefs.GetFloat("focus_lvl" + id.ToString(), 0f);
		int tours = PlayerPrefs.GetInt("tours_lvl" + id.ToString(), 0);
		int degats = PlayerPrefs.GetInt("degats_lvl" + id.ToString(), 0);

		focusText.text = focus.ToString() + "s";
		toursText.text = tours.ToString() + " tours";
		degatText.text = degats.ToString() + " dgt";
		
	}
}
