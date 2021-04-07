using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ennemyBasic : MonoBehaviour
{
	NavMeshAgent navMeshAgent;
	bool isMoving = false;

	public GameObject player;

	public int maxHealth = 50;
	int health = 50;
	public GameObject sliderVie;


	private Animator animationEnnemy;


	bool isPoisoned;
	float timerPoison = 0f;
	float speed = 0f;
	public bool isThisEnnemyTurn;
	

	AudioSource audioSource;
	public AudioClip audioOuch;
	public AudioClip audioAttack;
	public AudioClip audioMortEnnemy;


	public Collider ennemyCollider;
	
	Rigidbody[] ragdollRBs;
	Collider[] ragdollColliders;

	/// <summary>
	/// On stock les components dans des variables
	/// </summary>
	void Start()
    {
		ragdollRBs = GetComponentsInChildren<Rigidbody>();
		ragdollColliders = GetComponentsInChildren<Collider>();
		audioSource = GetComponent<AudioSource>();
	}

	/// <summary>
	/// Initialisation de certaines variables.
	/// </summary>
	private void Awake()
	{
		navMeshAgent = GetComponent<NavMeshAgent>();
	
		animationEnnemy = GetComponent<Animator>();
		speed = navMeshAgent.speed;
		health = maxHealth;
	}

	/// <summary>
	/// Quand il s'agit de son tour, commence à bouger
	/// </summary>
	void Update()
    {
		
		if (isMoving == false && GameManager.singleton.getPlayerTurn() == false && isThisEnnemyTurn)
		{

			StartCoroutine(Mouvement());
		}

		
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
		isMoving = true;
		navMeshAgent.isStopped = false;
		
		navMeshAgent.SetDestination(player.transform.position);
		

		//Déplace le personnage et lui inflige des dégats s'il est empoisonés. Ne s'arrête pas tant qu'il n'est pas à destination 
		//ou que le timer arrive à 0.
		while (navMeshAgent.pathPending || (navMeshAgent.remainingDistance > 3f && GameManager.singleton.getTimerEnnemy() > 0.2f))
		{
			Debug.Log(navMeshAgent.remainingDistance);
		

			if (isPoisoned)
			{
				timerPoison += Time.deltaTime;

				if (timerPoison > 0.8)
				{
					dealDamage(8);
					timerPoison = 0;
				}
			}
			yield return null;
		}

		

		//S'il est assez proche, il va attaquer le joueur
		float timerAttack = 0f;
		if (Vector3.Distance(player.transform.position, transform.position) <= 3)
		{
			navMeshAgent.isStopped = true;
			navMeshAgent.ResetPath();
			transform.LookAt(player.transform.position);
			animationEnnemy.SetBool("Running", false);
			GameManager.singleton.StartAttack(0); //Il s'agit d'un ennemi, il ne consomme pas de temps. Ne fait que s'assurer que le timer ne cause pas
												 //de bug
			while(timerAttack < 1f)
			{
				timerAttack += Time.deltaTime;
				yield return null;
			}

			audioSource.PlayOneShot(audioAttack);
			animationEnnemy.SetTrigger("Attack");
			player.GetComponent<Animator>().SetTrigger("Hurt");
			while (timerAttack < 3f)
			{
				
				timerAttack += Time.deltaTime;
				yield return null;
			}
			player.GetComponent<player>().damage(10);
			
			GameManager.singleton.FinishAttack();
			
		}

		navMeshAgent.isStopped = true;
		navMeshAgent.ResetPath();
		animationEnnemy.SetBool("Running", false);


		GameManager.singleton.changeTurn();
		isMoving = false;
	}

	/// <summary>
	/// Inflige des dégats à l'ennemi et le tue s'il se retrouve à 0
	/// </summary>
	/// <param name="damage">Les dégats à infliger</param>
	public void dealDamage(int damage)
	{
		health -= damage;
		sliderVie.GetComponent<Slider>().value = (float)health / (float)maxHealth * 100f;

		audioSource.PlayOneShot(audioOuch);


		//Si l'ennemi est mort, le transforme en ragdoll.
		if (health <= 0)
		{
			//Afin d'éviter des bugs
			StopAllCoroutines();

			//Audio de la mort
			audioSource.Stop();
			audioSource.PlayOneShot(audioMortEnnemy);

			//Désactive tout
			animationEnnemy.enabled = false;
			navMeshAgent.enabled = false;
			ennemyCollider.enabled = false;
			GameManager.singleton.killEnnemy(transform);

			//Active le ragdoll
			foreach (Collider rbcollider in ragdollColliders)
			{
				rbcollider.enabled = true;
			}

			foreach (Rigidbody rb in ragdollRBs)
			{
				rb.isKinematic = false;
			}

			//Continue à désactiver
			sliderVie.SetActive(false);
			this.enabled = false;

		}
	}

	/// <summary>
	/// Quand l'ennemi entre dans une zone trigger, permet d'infliger des dégats ou d'ajouter certains effets.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		//Ajoute le poison
		if (other.tag == "LentPoison")
		{
			navMeshAgent.speed = speed / 2;
			isPoisoned = true;
		}

		//Inflige les dégats
		if(other.tag == "attaqueJoueur")
		{
			dealDamage(other.GetComponent<PlayerAttaque>().damage);
		}
	}

	/// <summary>
	/// Quand l'ennemi sort d'une zone trigger, permet d'enlever certains effets
	/// </summary>
	/// <param name="other">Le trigger</param>
	private void OnTriggerExit(Collider other)
	{
		//Enlève le poison s'il sort d'une zone de poison. Remet la vitesse à la vitesse normale
		if (other.tag == "LentPoison")
		{
			navMeshAgent.speed = speed;
			isPoisoned = false;
		}
	}

}


