using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

	public static GameManager singleton;

	float timerJoueur = 0f;
	public float tempsTourJoueur = 10f;

	bool isPlayerTurn = true;
	
	bool isTimerStopped = false;

	float timerEnnemy = 0f;
	float tempsTourEnnemy = 3f;

	List<Transform> listeEnnemis = new List<Transform>();
	public GameObject conteneurEnnemi;
	int indexEnnemy = 0;

	/// <summary>
	/// Initialise le singleton s'il n'y en a pas déjà un.
	/// </summary>
	private void Awake()
	{
		if (singleton != null)
		{
			Debug.LogError("Détection de multiples instances du GameManager.");
			return;
		}

		// Assignation du singleton
		singleton = this;
	}

	/// <summary>
	/// Initialise la liste des ennemis et le timer
	/// </summary>
    private void Start()
    {
		timerJoueur = tempsTourJoueur;
		foreach (Transform child in conteneurEnnemi.transform)
			listeEnnemis.Add(child);
	}

	/// <summary>
	/// Change à qui appartient le tour. S'il s'agit du tour ennemi, 
	/// prend le tour de tous les ennemis avant de changer vers le tour du joueur.
	/// </summary>
    public void changeTurn()
	{
		//Tour joueur vers tour ennemi
		if(isPlayerTurn == true)
		{
			isPlayerTurn = false;
			UI_Manager.singleton.changeTurnText(false);
			timerEnnemy = tempsTourEnnemy;
			isTimerStopped = false;
		}
		
		//Tour ennemi vers autre ennemi ou joueur
		if(isPlayerTurn == false)
		{
			//Fait en sorte que l'ennemi précédent ne peux plus bouger
			if (indexEnnemy > 0)
			{
				if(listeEnnemis[indexEnnemy - 1] != null)
				{
					listeEnnemis[indexEnnemy - 1].GetComponent<ennemyBasic>().isThisEnnemyTurn = false;
				}
				
			}

			//S'il s'agissait du tour du dernier ennemi, change pour le tour du joueur
			if (indexEnnemy >= listeEnnemis.Count)
			{
				timerJoueur = tempsTourJoueur;
				isPlayerTurn = true;
				UI_Manager.singleton.changeTurnText(true);
				isTimerStopped = false;
				indexEnnemy = 0;
			}
			else
			{
				//Change l'ennemi actif.
				timerEnnemy = tempsTourEnnemy;
				

				listeEnnemis[indexEnnemy].GetComponent<ennemyBasic>().isThisEnnemyTurn = true;
				indexEnnemy += 1;
			}
			
		}
	}

	/// <summary>
	/// Retourne à qui est le tour
	/// </summary>
	/// <returns></returns>
	public bool getPlayerTurn()
	{
		return isPlayerTurn;
	}

	/// <summary>
	/// Stop le timer 
	/// </summary>
	/// <param name="cost"></param>
	public void StartAttack(float cost)
	{
		isTimerStopped = true;
		timerJoueur -= cost;
		UI_Manager.singleton.UpdateTimer(timerJoueur);
	}

	/// <summary>
	/// Réactive le timer.
	/// </summary>
	public void FinishAttack()
	{
		isTimerStopped = false;
	}
	
	/// <summary>
	/// Met à jour le bon timer (Joueur ou ennemi) et change le tour si le timer arrive à 0
	/// </summary>
    public void Update()
    {
        if (isPlayerTurn && isTimerStopped == false)
        {
			timerJoueur -= Time.deltaTime;

			UI_Manager.singleton.UpdateTimer(timerJoueur);
			if (timerJoueur <= 0f)
            {
				changeTurn();
            }
        }else if(isPlayerTurn == false && isTimerStopped == false)
        {
			timerEnnemy -= Time.deltaTime;

			UI_Manager.singleton.UpdateTimer(timerEnnemy);
			if (timerEnnemy <= 0f)
			{
				changeTurn();
			}

		}
    }

	/// <summary>
	/// Retourne le temps restant du joueur
	/// </summary>
	/// <returns></returns>
	public float getTimerJoueur()
    {
		return timerJoueur;
    }

	/// <summary>
	/// Retourne le temps restant des ennemis.
	/// </summary>
	/// <returns></returns>
	public float getTimerEnnemy()
	{
		return timerEnnemy;
	}

	/// <summary>
	/// Enlève un ennemi de la liste s'il est tué. Attend le tour du joueur s'il est tué durant le tour des ennemis.
	/// </summary>
	/// <param name="ennemy"></param>
	public void killEnnemy(Transform ennemy)
	{
		if (isPlayerTurn)
		{
			listeEnnemis.Remove(ennemy);
		}
		else
		{
			StartCoroutine(WaitForPlayerTurn(ennemy));
		}
		
	}

	/// <summary>
	/// Attend que ce soit au tour du joueur avant de supprimer l'ennemi de la liste
	/// </summary>
	/// <param name="ennemy">L'ennemi à supprimer</param>
	/// <returns></returns>
	IEnumerator WaitForPlayerTurn(Transform ennemy)
	{
		while(isPlayerTurn == false)
		{
			yield return null;
		}

		listeEnnemis.Remove(ennemy);

	}
}
