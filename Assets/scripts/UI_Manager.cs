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
}
