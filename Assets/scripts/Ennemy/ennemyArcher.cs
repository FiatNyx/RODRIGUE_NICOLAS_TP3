using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ennemyArcher : MonoBehaviour
{
	NavMeshAgent navMeshAgent;

	private Animator animationEnnemy;
	ennemyBasic scriptBase;
	AudioSource audioSource;
	public AudioClip audioAttack;
	public GameObject fireball;
	public Transform fireballPosition;
	

	/// <summary>
	/// Va chercher les component requis.
	/// </summary>
	void Start()
	{
		scriptBase = GetComponent<ennemyBasic>();
		audioSource = GetComponent<AudioSource>();
		animationEnnemy = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();


		//Enlever les commentaires pour faire en sorte que la difficulté affecte la vie de l'ennemi
		/*
		switch (DataManager.singleton.difficulte)
		{
			case "Facile":
				scriptBase.maxHealth = 10;
				break;
			case "Normal":
				scriptBase.maxHealth = 20;
				break;
			case "Difficile":
				scriptBase.maxHealth = 30;
				break;
			default:
				break;
		}

		scriptBase.health = scriptBase.maxHealth;
		*/
	}

	/// <summary>
	/// S'occupe des mouvements et de l'attaque de l'ennemi
	/// </summary>
	/// <returns></returns>
	IEnumerator Mouvement() 
	{
		//Animation
		animationEnnemy.SetBool("Running", true);


		//Se déplacer vers la destination
		scriptBase.isMoving = true;
		navMeshAgent.isStopped = false;


		
		//Détermine le joueur le plus proche et calcule une trajectoire pour fuir.
		Transform ennemyChoisi = scriptBase.getJoueurProche();
		
		Vector3 toPlayer = ennemyChoisi.position - transform.position;
		Vector3 targetPosition = toPlayer.normalized * -10f;
		
		navMeshAgent.SetDestination(transform.position + targetPosition);

		//Déplace le personnage et lui inflige des dégats s'il est empoisonés. Ne s'arrête pas tant qu'il n'est pas à destination 
		//ou que le timer arrive à 0.
		while ((navMeshAgent.pathPending || (Vector3.Distance(ennemyChoisi.position, transform.position) < 15f && navMeshAgent.remainingDistance > 1f)) && GameManager.singleton.getTimerEnnemy() > 0.2f)
		{
			if (scriptBase.isPoisoned > 0)
			{
				scriptBase.timerPoison += Time.deltaTime;

				if (scriptBase.timerPoison > 0.5)
				{
					scriptBase.dealDamage(scriptBase.puissancePoison);
					if (scriptBase.isDead)
					{
						yield break;
					}
					scriptBase.timerPoison = 0;
				}
			}
			yield return null;
		}


		//S'il est assez proche, il va attaquer le joueur
		float timerAttack = 0f;
		if (Vector3.Distance(ennemyChoisi.position, transform.position) < 20f)
		{
			//Réinitialise le navMeshAgent
			navMeshAgent.SetDestination(transform.position);
			navMeshAgent.isStopped = true;
			
			//Regarde vers la cible
			transform.LookAt(ennemyChoisi.position);
			fireballPosition.LookAt(ennemyChoisi.position + transform.up * 1.2f);
			
			//Animation
			animationEnnemy.SetBool("Running", false);
			GameManager.singleton.StartAttack(0); //Il s'agit d'un ennemi, il ne consomme pas de temps. Ne fait que s'assurer que le timer ne cause pas
												  //de bug
			while (timerAttack < 1f)
			{
				timerAttack += Time.deltaTime;
				yield return null;
			}

			//Son de l'attaque
			audioSource.PlayOneShot(audioAttack);
			
			//Lance la boule de feu.
			Instantiate(fireball, fireballPosition.position, fireballPosition.rotation);

			//Attend que la boule de feu finisse
			while (timerAttack < 5f)
			{

				timerAttack += Time.deltaTime;
				yield return null;
			}
			

			//Dépause le timer
			GameManager.singleton.FinishAttack();

		}

		//Arrête le navmesh, même s'il n'a pas attaqué
		navMeshAgent.isStopped = true;
		navMeshAgent.ResetPath();
		animationEnnemy.SetBool("Running", false);

		//Change le tour
		GameManager.singleton.changeTurn();
		scriptBase.isMoving = false;
	}

	/// <summary>
	/// Quand il s'agit de son tour, commence à bouger
	/// </summary>
	private void Update()
	{
		//S'il peut faire son attaque
		if (scriptBase.isMoving == false && GameManager.singleton.getPlayerTurn() == false && scriptBase.isThisEnnemyTurn && scriptBase.isDead == false && GameManager.singleton.isPaused == false)
		{

			StartCoroutine(Mouvement());
		}
	}
}
