using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ennemyHealer : MonoBehaviour
{
	NavMeshAgent navMeshAgent;
	
	private Animator animationEnnemy;
	ennemyBasic scriptBase;
	AudioSource audioSource;
	public AudioClip audioAttack;
	public GameObject particleHealPrefab;

	// Start is called before the first frame update
	void Start()
    {
		scriptBase = GetComponent<ennemyBasic>();
		audioSource = GetComponent<AudioSource>();
		animationEnnemy = GetComponent<Animator>();
		navMeshAgent = GetComponent<NavMeshAgent>();

		/*
		switch (DataManager.singleton.difficulte)
		{
			case "Facile":
				scriptBase.maxHealth = 20;
				break;
			case "Normal":
				scriptBase.maxHealth = 25;
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
	IEnumerator Mouvement() //Changer mouvement pour les autres types d'ennemis, genre le mettre dans un autre component
	{
		List<Transform> listeEnnemy = GameManager.singleton.listeEnnemis;

		Transform ennemyFaible = transform;
		int vieEnnemyFaible = 1000;
        foreach (var ennemy in listeEnnemy)
        {
			if(ennemy.GetComponent<ennemyBasic>().getVie() < vieEnnemyFaible && ennemy.GetComponent<ennemyBasic>().isDead == false)
            {
				vieEnnemyFaible = ennemy.GetComponent<ennemyBasic>().getVie();
				ennemyFaible = ennemy;
            }
        }

		//Animation
		animationEnnemy.SetBool("Running", true);


		//Me déplacer vers la destination
		scriptBase.isMoving = true;
		navMeshAgent.isStopped = false;

		navMeshAgent.SetDestination(ennemyFaible.position);


		//Déplace le personnage et lui inflige des dégats s'il est empoisonés. Ne s'arrête pas tant qu'il n'est pas à destination 
		//ou que le timer arrive à 0.
		while (navMeshAgent.pathPending || (navMeshAgent.remainingDistance > 10f && GameManager.singleton.getTimerEnnemy() > 0.2f))
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
		if (Vector3.Distance(ennemyFaible.position, transform.position) <= 15)
		{
			navMeshAgent.isStopped = true;
			navMeshAgent.ResetPath();
			transform.LookAt(ennemyFaible.position);
			animationEnnemy.SetBool("Running", false);
			GameManager.singleton.StartAttack(0); //Il s'agit d'un ennemi, il ne consomme pas de temps. Ne fait que s'assurer que le timer ne cause pas
												  //de bug
			while (timerAttack < 1f)
			{
				timerAttack += Time.deltaTime;
				yield return null;
			}

			audioSource.PlayOneShot(audioAttack);
			animationEnnemy.SetTrigger("Heal");
			
			while (timerAttack < 3f)
			{

				timerAttack += Time.deltaTime;
				yield return null;
			}

			GameObject particleHeal = Instantiate(particleHealPrefab, ennemyFaible.transform.position, ennemyFaible.transform.rotation);
			ennemyFaible.GetComponent<ennemyBasic>().heal(10);

			yield return new WaitForSeconds(1.3f);
			Destroy(particleHeal);
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
		if (scriptBase.isMoving == false && GameManager.singleton.getPlayerTurn() == false && scriptBase.isThisEnnemyTurn && scriptBase.isDead == false && GameManager.singleton.isPaused == false)
		{

			StartCoroutine(Mouvement());
		}
	}
}
