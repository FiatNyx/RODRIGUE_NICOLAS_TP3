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
	// Start is called before the first frame update
	void Start()
	{
		scriptBase = GetComponent<ennemyBasic>();
		audioSource = GetComponent<AudioSource>();
		animationEnnemy = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();
	}

	/// <summary>
	/// S'occupe des mouvements et de l'attaque de l'ennemi
	/// </summary>
	/// <returns></returns>
	IEnumerator Mouvement() //Changer mouvement pour les autres types d'ennemis, genre le mettre dans un autre component
	{
		//Animation
		animationEnnemy.SetBool("Running", true);


		//Me déplacer vers la destination
		scriptBase.isMoving = true;
		navMeshAgent.isStopped = false;






		//Déplace le personnage et lui inflige des dégats s'il est empoisonés. Ne s'arrête pas tant qu'il n'est pas à destination 
		//ou que le timer arrive à 0.

		Transform ennemyChoisi = scriptBase.getJoueurProche();
		Debug.Log(ennemyChoisi);
		Vector3 toPlayer = ennemyChoisi.position - transform.position;
		Vector3 targetPosition = toPlayer.normalized * -10f;
		//Vector3 vecteurRandom = new Vector3(Random.Range(-50, 50), 0, Random.Range(-50, 50));
		navMeshAgent.SetDestination(transform.position + targetPosition);
		

		while ((navMeshAgent.pathPending || Vector3.Distance(ennemyChoisi.position, transform.position) < 15f) && GameManager.singleton.getTimerEnnemy() > 0.2f)
		{
			


			if (scriptBase.isPoisoned > 0)
			{
				scriptBase.timerPoison += Time.deltaTime;

				if (scriptBase.timerPoison > 0.8)
				{
					scriptBase.dealDamage(8);
					scriptBase.timerPoison = 0;
				}
			}
			yield return null;
		}


		//S'il est assez proche, il va attaquer le joueur
		float timerAttack = 0f;
		if (Vector3.Distance(ennemyChoisi.position, transform.position) < 20f)
		{
			navMeshAgent.SetDestination(transform.position);
			navMeshAgent.isStopped = true;
			
			transform.LookAt(ennemyChoisi.position);
			fireballPosition.LookAt(ennemyChoisi.position);
			//Marche pas, il le fait pas en y
			
			animationEnnemy.SetBool("Running", false);
			GameManager.singleton.StartAttack(0); //Il s'agit d'un ennemi, il ne consomme pas de temps. Ne fait que s'assurer que le timer ne cause pas
												  //de bug
			while (timerAttack < 1f)
			{
				timerAttack += Time.deltaTime;
				yield return null;
			}

			audioSource.PlayOneShot(audioAttack);
			//animationEnnemy.SetTrigger("Attack");
			//scriptBase.player.GetComponent<Animator>().SetTrigger("Hurt");
			Instantiate(fireball, fireballPosition.position, fireballPosition.rotation);
			while (timerAttack < 5f)
			{

				timerAttack += Time.deltaTime;
				yield return null;
			}
			

			GameManager.singleton.FinishAttack();

		}

		navMeshAgent.isStopped = true;
		navMeshAgent.ResetPath();
		animationEnnemy.SetBool("Running", false);


		GameManager.singleton.changeTurn();
		scriptBase.isMoving = false;
	}

	/// <summary>
	/// Quand il s'agit de son tour, commence à bouger
	/// </summary>
	private void Update()
	{
		if (scriptBase.isMoving == false && GameManager.singleton.getPlayerTurn() == false && scriptBase.isThisEnnemyTurn)
		{

			StartCoroutine(Mouvement());
		}
	}
}
