using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ennemyBasic : MonoBehaviour
{
	public NavMeshAgent navMeshAgent;

	public GameObject player;

	public int maxHealth = 50;
	int health = 50;
	public GameObject sliderVie;

	private Animator animationEnnemy;
	public bool isMoving = false;

	public int isPoisoned;
	public float timerPoison = 0f;
	public int puissancePoison = 0;
	public int burnStatus = 0;
	public float speed = 0f;

	public bool isThisEnnemyTurn;
	public bool isDead = false;

	AudioSource audioSource;
	public AudioClip audioOuch;
	public AudioClip audioMortEnnemy;

	public Collider ennemyCollider;
	Rigidbody[] ragdollRBs;
	Collider[] ragdollColliders;
	
	public GameObject brulureParticules;



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


    private void Update()
    {
        if (isThisEnnemyTurn)
        {
			//S'assure que la caméra suit le personnage
			GameManager.singleton.cameraPosition.position = transform.position;
		}
    }

	/// <summary>
	/// Au début du tour, inflige les effets de status
	/// </summary>
	public void UpdateStatus()
    {
		//Inflige les dégâts de brulure
		if (burnStatus > 0)
		{
			int previousBurn = burnStatus;
			burnStatus =  (int)Mathf.Floor(burnStatus / 2f);

			dealDamage((previousBurn - burnStatus) * 10);
			if (burnStatus <= 0)
			{
				brulureParticules.SetActive(false);
				burnStatus = 0;
			}
		}
		
	}

	/// <summary>
	/// Inflige une brulure à l'ennemi
	/// </summary>
	/// <param name="burnAmount">Le nombre de brulures à infliger</param>
	public void Enflammer(int burnAmount)
    {
		brulureParticules.SetActive(true);
		burnStatus += burnAmount;
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
			isDead = true;

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
			isDead = true;

		}
	}

	/// <summary>
	/// Quand l'ennemi entre dans une zone trigger, permet d'infliger des dégats ou d'ajouter certains effets.
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		
		if (isDead == false)
        {
			
			//Inflige les dégats
			if (other.tag == "attaqueJoueur")
			{
				print("Dégat from attaque joueur");
				dealDamage(other.GetComponent<Attaque>().damage);
			}

			//Ralentit
			if (other.GetComponent<zoneLente>() != null)
			{
				print("Slow from slow");
				navMeshAgent.speed = speed / other.GetComponent<zoneLente>().getSlowStrength(); 
			}

			//Inflige du poison
			if (other.GetComponent<zonePoison>() != null)
			{
				print("Poison from zone poison");
				print(other.GetComponent<zonePoison>().getPoisonStrength());
				puissancePoison = other.GetComponent<zonePoison>().getPoisonStrength();
				isPoisoned += 1;
			}

			//Inflige des dégâts par l'attaque de l'explosion
			if (other.GetComponent<ExplosionCircle>() != null)
			{
				if (other.GetComponent<ExplosionCircle>().isDamage == true)
				{
					dealDamage(other.GetComponent<ExplosionCircle>().damageAmount);
				}
			}

			//Inflige des brulures par les attaques qui en inflige.
			if(other.GetComponent<FeuStatusAttack>() != null)
            {
				Enflammer(other.GetComponent<FeuStatusAttack>().burnAmount);
            }
		}
		
	}

	/// <summary>
	/// Quand l'ennemi sort d'une zone trigger, permet d'enlever certains effets
	/// </summary>
	/// <param name="other">Le trigger</param>
	private void OnTriggerExit(Collider other)
	{
		if(isDead == false)
        {
			
			//Enlève le ralentissement
			if (other.GetComponent<zoneLente>() != null)
			{
				navMeshAgent.speed = speed;

			}

			//Enlève le poison
			if (other.GetComponent<zonePoison>() != null)
			{
				isPoisoned = 0;
				
			}
		}

		
	}

	/// <summary>
	/// Retourne la vie
	/// </summary>
	/// <returns>La vie de cet ennemi</returns>
	public int getVie()
    {
		return health;
    }


	/// <summary>
	/// Soigne l'ennemi d'un certain montant
	/// </summary>
	/// <param name="healAmount">Le montant de vie à soigner</param>
	public void heal(int healAmount)
    {
		if(isDead == false)
        {
			health += healAmount;
			if (health > maxHealth)
			{
				health = maxHealth;
			}

			sliderVie.GetComponent<Slider>().value = (float)health / (float)maxHealth * 100f;
		}
	}

	/// <summary>
	/// La fonction qui permet de trouver le joueur le plus proche
	/// </summary>
	/// <returns>Le joueur le plus proche</returns>
	public Transform getJoueurProche()
    {
		Transform joueurChoisi = GameManager.singleton.listeJoueurs[0];
		float distance = Mathf.Abs(Vector3.Distance(transform.position, joueurChoisi.position));

        for (int i = 0; i < GameManager.singleton.listeJoueurs.Count; i++)
        {
			if (Mathf.Abs(Vector3.Distance(transform.position, GameManager.singleton.listeJoueurs[i].position)) < distance && GameManager.singleton.listeJoueurs[i].GetComponent<JoueurMain>().isDead == false)
            {
				joueurChoisi = GameManager.singleton.listeJoueurs[i];
				distance = Mathf.Abs(Vector3.Distance(transform.position, joueurChoisi.position));

			}
        }
		return joueurChoisi;
    }

}


