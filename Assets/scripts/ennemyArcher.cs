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
		while ((navMeshAgent.pathPending || Vector3.Distance(scriptBase.player.transform.position, transform.position) < 20) && GameManager.singleton.getTimerEnnemy() > 0.2f)
		{
			Vector3 toPlayer = scriptBase.player.transform.position - transform.position;
			if (Vector3.Distance(scriptBase.player.transform.position, transform.position) < 20) //MARCHE PAS MARCHE PAS
			{
				Vector3 targetPosition = toPlayer.normalized * -10f;
				navMeshAgent.SetDestination(targetPosition);

			}


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
		if (Vector3.Distance(scriptBase.player.transform.position, transform.position) < 20)
		{
			navMeshAgent.isStopped = true;
			navMeshAgent.ResetPath();
			transform.LookAt(scriptBase.player.transform.position); //Marche pas, il le fait pas en y
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
			Instantiate(fireball, transform.position + transform.forward * 3 + transform.up * 2, transform.rotation);
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
