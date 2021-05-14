using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

	public static GameManager singleton;

	public float timerJoueur = 0f;
	public float tempsTourJoueur = 10f;

	bool isPlayerTurn = true;

	bool isTimerStopped = false;

	float timerEnnemy = 0f;
	float tempsTourEnnemy = 3f;

	public List<Transform> listeEnnemis = new List<Transform>();
	public List<Transform> listeJoueurs = new List<Transform>();

	public GameObject conteneurEnnemi;
	public GameObject conteneurJoueurs;
	int indexEnnemy = 0;
	int indexJoueur = 1;

	public int levelSlow = 0;
	public int levelPoison = 0;
	public int levelHeal = 0;

	public int levelID = 1;
	public Transform cameraPosition;
	public Transform cameraViePosition;
	float timerChangeTurn = 0f;
	public float vitesseLerpChangeTurn = 2f;
	public List<ObjectTemporaire> listeObjectsTemporaires;

	int nbTours = 1;
	float tempsFocus = 0f;
	public int nbDegatTotal = 0;

	bool isStarted = false;
	public bool isPaused = false;

	/// <summary>
	/// Initialise le singleton s'il n'y en a pas déjà un.
	/// </summary>
	private void Awake()
	{
		Time.timeScale = 1;
		isPaused = false;
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
		{
			listeEnnemis.Add(child);
		}

		foreach (Transform child in conteneurJoueurs.transform)
		{
			listeJoueurs.Add(child);
			UI_Manager.singleton.listeJoueurs.Add(child);
		}
		
		UI_Manager.singleton.changeVieText();
		StartCoroutine(StartCombat());

	}


	IEnumerator StartCombat()
	{
		Transform joueurPrecedent = null;
		Transform premierPerso = null;

		foreach (var joueur in listeJoueurs)
		{
			if(joueurPrecedent == null)
			{
				joueurPrecedent = joueur;
				premierPerso = joueur;
				cameraPosition.position = joueur.position;
				yield return new WaitForSeconds(1f);
			}
			else
			{
				timerChangeTurn = vitesseLerpChangeTurn;
				while (timerChangeTurn > 0)
				{
					cameraPosition.position = Vector3.Lerp(joueurPrecedent.position, joueur.position, 1 - timerChangeTurn / vitesseLerpChangeTurn);
					timerChangeTurn -= Time.deltaTime;
					yield return null;
				}
				joueurPrecedent = joueur;
				yield return new WaitForSeconds(0.2f);
			}
		}

		foreach (var ennemi in listeEnnemis)
		{
			
			timerChangeTurn = vitesseLerpChangeTurn;
			while (timerChangeTurn > 0)
			{
				cameraPosition.position = Vector3.Lerp(joueurPrecedent.position, ennemi.position, 1 - timerChangeTurn / vitesseLerpChangeTurn);
				timerChangeTurn -= Time.deltaTime;
				yield return null;
			}
			joueurPrecedent = ennemi;
			yield return new WaitForSeconds(0.2f);
		}


		timerChangeTurn = vitesseLerpChangeTurn;
		while (timerChangeTurn > 0)
		{
			cameraPosition.position = Vector3.Lerp(joueurPrecedent.position, premierPerso.position, 1 - timerChangeTurn / vitesseLerpChangeTurn);
			timerChangeTurn -= Time.deltaTime;
			yield return null;
		}

		listeJoueurs[0].GetComponent<JoueurMain>().isThisPlayersTurn = true;
		UI_Manager.singleton.changeSelectedChar(0);

		isStarted = true;
	}

	/// <summary>
	/// Change à qui appartient le tour. S'il s'agit du tour ennemi, 
	/// prend le tour de tous les ennemis avant de changer vers le tour du joueur.
	/// </summary>
	public void changeTurn()
	{
		//Tour joueur vers tour ennemi
		if (isPlayerTurn == true && timerChangeTurn <= 0)
		{
			print(listeJoueurs);
			if (indexJoueur > 0)
			{
				if (listeJoueurs[indexJoueur - 1] != null)
				{
					listeJoueurs[indexJoueur - 1].GetComponent<JoueurMain>().isThisPlayersTurn = false;
					listeJoueurs[indexJoueur - 1].GetComponent<JoueurAttaques>().resetAttackSelected();

				}

			}

			//S'il s'agissait du tour du dernier ennemi, change pour le tour du joueur
			if (indexJoueur >= listeJoueurs.Count)
			{
				StartAttack(0);
				StartCoroutine(WaitForChangeTurn(true));

			}
			else
			{
				StartAttack(0);
				StartCoroutine(WaitForChangeTurn(false));

			}

		}
		
		//Tour ennemi vers autre ennemi ou joueur
		if (isPlayerTurn == false && timerChangeTurn <= 0)
		{
			//Fait en sorte que l'ennemi précédent ne peux plus bouger
			if (indexEnnemy > 0)
			{
				if (listeEnnemis[indexEnnemy - 1] != null)
				{
					listeEnnemis[indexEnnemy - 1].GetComponent<ennemyBasic>().isThisEnnemyTurn = false;
					
				}

			}

			//S'il s'agissait du tour du dernier ennemi, change pour le tour du joueur
			if (indexEnnemy >= listeEnnemis.Count)
			{
				StartAttack(0);
				StartCoroutine(WaitForChangeTurn(true));
			}
			else
			{
				StartAttack(0);
				StartCoroutine(WaitForChangeTurn(false));



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

	public void TogglePause()
	{
		if (isPaused == false)
		{
			Time.timeScale = 0;
			MusicManager.singleton.ToggleMusic();
			UI_Manager.singleton.ToggleMenuPause(true);
			isPaused = true;
		}
		else
		{
			Time.timeScale = 1;
			MusicManager.singleton.ToggleMusic();
			UI_Manager.singleton.ToggleMenuPause(false);
			isPaused = false;
		}
	}
	/// <summary>
	/// Met à jour le bon timer (Joueur ou ennemi) et change le tour si le timer arrive à 0
	/// </summary>
	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.P))
		{
			TogglePause();
			
		}
		if (isStarted && isPaused == false)
		{
			if (isPlayerTurn && isTimerStopped == false)
			{
				if (listeJoueurs[indexJoueur - 1].GetComponent<JoueurMovement>().isMoving == false && listeJoueurs[indexJoueur - 1].GetComponent<JoueurMain>().moveSelected == 0)
				{
					MusicManager.singleton.slowDownMusic();
					timerJoueur -= Time.deltaTime / 4;
					tempsFocus += Time.deltaTime;
				}
				else
				{
					MusicManager.singleton.normalMusicSpeed();
					timerJoueur -= Time.deltaTime;
				}



				UI_Manager.singleton.UpdateTimer(timerJoueur);
				if (timerJoueur <= 0f)
				{
					changeTurn();
				}
			}
			else if (isPlayerTurn == false && isTimerStopped == false)
			{
				timerEnnemy -= Time.deltaTime;

				UI_Manager.singleton.UpdateTimer(timerEnnemy);
				if (timerEnnemy <= 0f)
				{
					changeTurn();
				}

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
			if (listeEnnemis.Count <= 0)
			{
				Time.timeScale = 0;
				isPaused = true;
				MusicManager.singleton.ToggleMusic();

				UI_Manager.singleton.OuvrirMenuVictoire(levelID, tempsFocus, nbTours, nbDegatTotal);

				isPaused = true;
				float focus = PlayerPrefs.GetFloat("focus_lvl" + levelID.ToString(), 1000f);
				int tours = PlayerPrefs.GetInt("tours_lvl" + levelID.ToString(), 1000);
				int degats = PlayerPrefs.GetInt("degats_lvl" + levelID.ToString(), 1000);

				if(tempsFocus < focus)
				{
					PlayerPrefs.SetFloat("focus_lvl" + levelID.ToString(), tempsFocus);
				}

				if(nbTours < tours)
				{
					PlayerPrefs.SetInt("tours_lvl" + levelID.ToString(), nbTours);
				}
				
				if(nbDegatTotal < degats)
				{
					PlayerPrefs.SetInt("degats_lvl" + levelID.ToString(), nbDegatTotal);
				}

				PlayerPrefs.Save();

				
				
			}
		}
		else
		{
			StartCoroutine(WaitForPlayerTurn(ennemy));
		}

	}

	/// <summary>
	/// Enlève un ennemi de la liste s'il est tué. Attend le tour du joueur s'il est tué durant le tour des ennemis.
	/// </summary>
	/// <param name="ennemy"></param>
	public void killJoueur(Transform joueur)
	{
		if (isPlayerTurn == false)
		{
			int indexJoueur = listeJoueurs.IndexOf(joueur);
			UI_Manager.singleton.killJoueur(indexJoueur);
			listeJoueurs.Remove(joueur);
			if (listeJoueurs.Count <= 0)
			{
				Time.timeScale = 0;
				MusicManager.singleton.ToggleMusic();
				UI_Manager.singleton.OuvrirMenuDefaite();
				isPaused = true;

			}
		}
		else
		{
			timerJoueur = 1f;
			StartCoroutine(WaitForEnnemyTurn(joueur));
		}

	}

	/// <summary>
	/// Attend que ce soit au tour du joueur avant de supprimer l'ennemi de la liste
	/// </summary>
	/// <param name="ennemy">L'ennemi à supprimer</param>
	/// <returns></returns>
	IEnumerator WaitForPlayerTurn(Transform ennemy)
	{
		while (isPlayerTurn == false)
		{
			yield return null;
		}

		listeEnnemis.Remove(ennemy);
		if (listeEnnemis.Count <= 0)
		{
			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.name);
		}

	}

	IEnumerator LerpChangeTurn(Transform pointDepart, Transform pointFin)
	{

		while (timerChangeTurn > 0)
		{

			cameraPosition.position = Vector3.Lerp(pointDepart.position, pointFin.position, 1 - timerChangeTurn / vitesseLerpChangeTurn);
			timerChangeTurn -= Time.deltaTime;
			yield return null;
		}

	}


	IEnumerator WaitForChangeTurn(bool isFin)
	{
		if (isFin)
		{
			if (isPlayerTurn)
			{
				MusicManager.singleton.normalMusicSpeed();
				timerChangeTurn = vitesseLerpChangeTurn;
				StartCoroutine(LerpChangeTurn(listeJoueurs[listeJoueurs.Count - 1], listeEnnemis[0]));
				yield return new WaitForSeconds(vitesseLerpChangeTurn);
				isPlayerTurn = false;
				UI_Manager.singleton.changeTurnText(false);
				timerEnnemy = tempsTourEnnemy;
				isTimerStopped = false;
				indexJoueur = 0;
				indexEnnemy = 0;
				changeTurn();
			}
			else
			{
				timerChangeTurn = vitesseLerpChangeTurn;
				StartCoroutine(LerpChangeTurn(listeEnnemis[listeEnnemis.Count - 1], listeJoueurs[0]));
				yield return new WaitForSeconds(vitesseLerpChangeTurn);

				timerJoueur = tempsTourJoueur;
				isPlayerTurn = true;
				UI_Manager.singleton.changeTurnText(true);
				isTimerStopped = false;
				indexEnnemy = 0;

				foreach (ObjectTemporaire objectTemporaire in listeObjectsTemporaires)
				{
					objectTemporaire.UpdateObjet();
				}

				nbTours += 1;
				changeTurn(); 

			}
		}
		else
		{
			if (isPlayerTurn)
			{

				if (indexJoueur - 1 >= 0)
				{
					timerChangeTurn = vitesseLerpChangeTurn;
					StartCoroutine(LerpChangeTurn(listeJoueurs[indexJoueur - 1], listeJoueurs[indexJoueur]));
					yield return new WaitForSeconds(vitesseLerpChangeTurn);
				}


				//Change l'ennemi actif.
				timerJoueur = tempsTourJoueur;


				listeJoueurs[indexJoueur].GetComponent<JoueurMain>().isThisPlayersTurn = true;
				listeJoueurs[indexJoueur].GetComponent<JoueurMain>().updateEffets();
				listeJoueurs[indexJoueur].GetComponent<JoueurMain>().healedThisTurn = false;
				UI_Manager.singleton.changeSelectedChar(indexJoueur);
				
				indexJoueur += 1;

				FinishAttack();
			}
			else
			{


				if (indexEnnemy - 1 >= 0)
				{
					timerChangeTurn = vitesseLerpChangeTurn;
					StartCoroutine(LerpChangeTurn(listeEnnemis[indexEnnemy - 1], listeEnnemis[indexEnnemy]));
					yield return new WaitForSeconds(vitesseLerpChangeTurn);
				}


				//Change l'ennemi actif.
				timerEnnemy = tempsTourEnnemy;


				listeEnnemis[indexEnnemy].GetComponent<ennemyBasic>().isThisEnnemyTurn = true;
				listeEnnemis[indexEnnemy].GetComponent<ennemyBasic>().UpdateStatus();
				indexEnnemy += 1;
				FinishAttack();
			}
		}

	}

	/// <summary>
	/// Attend que ce soit au tour du joueur avant de supprimer l'ennemi de la liste
	/// </summary>
	/// <param name="ennemy">L'ennemi à supprimer</param>
	/// <returns></returns>
	IEnumerator WaitForEnnemyTurn(Transform joueur)
	{
		while (isPlayerTurn == true)
		{
			yield return null;
		}
		int indexJoueur = listeJoueurs.IndexOf(joueur);
		UI_Manager.singleton.killJoueur(indexJoueur);
		listeJoueurs.Remove(joueur);
		if (listeJoueurs.Count <= 0)
		{
			Time.timeScale = 0;
			MusicManager.singleton.ToggleMusic();
			UI_Manager.singleton.OuvrirMenuDefaite();
			isPaused = true;
		}

	}


	public void changeLevelPoison(int change)
	{
		levelPoison += change;

		if (levelPoison > 3)
		{
			levelPoison = 3;
		}
		else if (levelPoison < 0)
		{
			levelPoison = 0;
		}
	}

	public void changeLevelSlow(int change)
	{
		levelSlow += change;

		if (levelSlow > 3)
		{
			levelSlow = 3;
		}
		else if (levelSlow < 0)
		{
			levelSlow = 0;
		}
	}

	public void changeLevelHeal(int change)
    {
		levelHeal += change;

		if(levelHeal > 3)
        {
			levelHeal = 3;
        }
		else if(levelHeal < 0)
        {
			levelHeal = 0;
        }
    }


}
