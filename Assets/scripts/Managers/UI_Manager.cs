using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour
{
    public static UI_Manager singleton;
    public Text textTour;
	public Text textTimer;
	public Text textVie;
	public List<RawImage> uiAttaques;
	public List<RawImage> listeSelectedUI;
	public List<Transform> listeJoueurs;
	public GameObject listeAttaqueP1;
	public GameObject listeAttaqueP2;
	public GameObject menuPause;
	public GameObject menuVictoire;
	public GameObject menuDefaite;


	public Text focusText;
	public Text toursText;
	public Text degatText;

	int indexJoueur = 0;
	List<GameObject> listeAttaques;
	/// <summary>
	/// Assignation du singleton s'il n'en a pas déjà un.
	/// </summary>
	private void Awake()
    {
        if (singleton != null)
        {
            Debug.LogError("Détection de multiples instances du GameManager.");
            return;
        }
		singleton = this;

		listeJoueurs = new List<Transform>();
        listeAttaques = new List<GameObject>
        {
            listeAttaqueP1,
            listeAttaqueP2
        };

    }


	public void ToggleMenuPause(bool active)
	{
		menuPause.SetActive(active);
	}
  

	/// <summary>
	/// Ouvre le menu victoire et affiche le score. Détermine aussi s'il y a un nouveau record
	/// </summary>
	/// <param name="levelID"></param>
	/// <param name="focus"></param>
	/// <param name="tours"></param>
	/// <param name="degats"></param>
	public void OuvrirMenuVictoire(int levelID, float focus, int tours, int degats)
	{
		menuVictoire.SetActive(true);
		float OldFocus = PlayerPrefs.GetFloat("focus_lvl" + levelID.ToString().ToString() + DataManager.singleton.difficulte, 10000f);
		int OldTours = PlayerPrefs.GetInt("tours_lvl" + levelID.ToString().ToString() + DataManager.singleton.difficulte, 10000);
		int OldDegats = PlayerPrefs.GetInt("degats_lvl" + levelID.ToString().ToString() + DataManager.singleton.difficulte, 10000);



		focusText.text = focus.ToString() + "s";

		if(OldFocus > focus)
		{
			focusText.text = focusText.text + "   (Nouveau record!)";
		}

		toursText.text = tours.ToString() + " tours";
		if (OldTours > tours)
		{
			toursText.text = toursText.text + "   (Nouveau record!)";
		}



		degatText.text = degats.ToString() + " dgt";
		if (OldDegats > degats)
		{
			degatText.text = degatText.text + "   (Nouveau record!)";
		}
	}


	public void OuvrirMenuDefaite()
	{
		menuDefaite.SetActive(true);
	}

    /// <summary>
    /// Reçoit le temps restant et l'affiche dans l'UI
    /// </summary>
    /// <param name="timeRemaining">Le temps restant au tour actuel.</param>
    public void UpdateTimer(float timeRemaining)
    {
        timeRemaining = Mathf.Round(timeRemaining * 10f) / 10f;
        textTimer.text = timeRemaining.ToString();
    }

	/// <summary>
	/// Change à qui appartient le tour dans l'UI
	/// </summary>
	/// <param name="isTurnJoueur">True s'il s'agit du tour du joueur, false s'il s'agit du tour des ennemis</param>
    public void changeTurnText(bool isTurnJoueur)
    {
		if(isTurnJoueur)
		{
			textTour.text = "Tour du joueur";
		}
		else
		{
			textTour.text = "Tour des ennemis";
		}
    }

	/// <summary>
	/// Met à jour la vie du joueur dans l'UI
	/// </summary>
	/// <param name="vieMax">La vie maximale du personnage</param>
	/// <param name="vie">La vie actuelle du personnage</param>
	public void changeVieText()
	{
		string stringVie = "";

		
        foreach (var joueur in listeJoueurs)
        {
			JoueurMain joueurScript = joueur.GetComponent<JoueurMain>();
			stringVie += joueur.name + " vie :  " + joueurScript.vie.ToString() + "/" + joueurScript.vieMax.ToString() + "\n";
		}
		textVie.text = stringVie;
	}

	/// <summary>
	/// Affiche un contour jaune sur la compétence actuellement sélectionnée
	/// </summary>
	/// <param name="index">L'index de la compétence, 0 = aucune compétence</param>
	public void changeSelectedMove(int index)
	{
		//Déselectionner les autres compétences dans l'UI
		foreach (RawImage image in listeSelectedUI)
		{
			image.enabled = false;
		}

		
		if(index != 0)
		{
			listeSelectedUI[index - 1].enabled = true;
		}
		
	}

	/// <summary>
	/// Change les icones selon le personnage qui est en train de jouer
	/// </summary>
	/// <param name="index">Le personnage actif</param>
	public void changeSelectedChar(int index)
    {
        foreach (GameObject listeAttaqe in listeAttaques)
        {
			listeAttaqe.SetActive(false);
        }
		changeSelectedMove(0);
		listeAttaques[index].SetActive(true);
		indexJoueur = index;
    }

	/// <summary>
	/// Supprime les icones d'un personnage de la liste
	/// </summary>
	/// <param name="index">Le personnage qui a été tué</param>
	public void killJoueur(int index)
    {
		foreach (GameObject listeAttaqe in listeAttaques)
		{
			listeAttaqe.SetActive(false);
		}
		changeSelectedMove(0);
		listeAttaques.RemoveAt(index);
    }
}
