using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ennemyBasic : MonoBehaviour
{
	public NavMeshAgent navMeshAgent;

	public bool isMoving = false;
	public GameObject player;

	public int maxHealth = 50;
	int health = 50;
	public GameObject sliderVie;


	private Animator animationEnnemy;


	public int isPoisoned;
	public float timerPoison = 0f;
	public float speed = 0f;
	public bool isThisEnnemyTurn;
	

	AudioSource audioSource;
	public AudioClip audioOuch;
	
	public AudioClip audioMortEnnemy;

	
	public Collider ennemyCollider;
	
	Rigidbody[] ragdollRBs;
	Collider[] ragdollColliders;
	public bool isDead = false;
	public int puissancePoison = 0;

	public int burnStatus = 0;
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


	public void UpdateStatus()
    {
		if (burnStatus > 0)
		{
			int previousBurn = burnStatus;
			burnStatus = Mathf.RoundToInt(burnStatus / 2.1f);

			dealDamage((previousBurn - burnStatus) * 10);
		}

		print("Burning");
	}

	public void Enflammer(int burnAmount)
    {
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

			if (other.GetComponent<zoneLente>() != null)
			{
				print("Slow from slow");
				navMeshAgent.speed = speed / other.GetComponent<zoneLente>().getSlowStrength(); 

			}

			if (other.GetComponent<zonePoison>() != null)
			{
				print("Poison from zone poison");
				print(other.GetComponent<zonePoison>().getPoisonStrength());
				puissancePoison = other.GetComponent<zonePoison>().getPoisonStrength();
				isPoisoned += 1;
			}

			if (other.GetComponent<ExplosionCircle>() != null)
			{
				print("Damage from explosion");
				if (other.GetComponent<ExplosionCircle>().isDamage == true)
				{
					dealDamage(other.GetComponent<ExplosionCircle>().damageAmount);
				}
			}

			
			if(other.GetComponent<FeuStatusAttack>() != null)
            {
				Enflammer(other.GetComponent<FeuStatusAttack>().burnAmount);
				print(burnStatus);
            }
		}
		
	}

	/// <summary>
	/// Quand l'ennemi sort d'une zone trigger, permet d'enlever certains effets
	/// </summary>
	/// <param name="other">Le trigger</param>
	private void OnTriggerExit(Collider other)
	{
		print("exit");
		if(isDead == false)
        {
			

			if (other.GetComponent<zoneLente>() != null)
			{
				navMeshAgent.speed = speed;

			}

			if (other.GetComponent<zonePoison>() != null)
			{
				isPoisoned = 0;
				
			}
		}

		
	}

	public int getVie()
    {
		return health;
    }

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

	public Transform getJoueurProche()
    {

		Transform joueurChoisi = GameManager.singleton.listeJoueurs[0];
		float distance = Mathf.Abs(Vector3.Distance(transform.position, joueurChoisi.position));

        for (int i = 0; i < GameManager.singleton.listeJoueurs.Count; i++)
        {
			if (Mathf.Abs(Vector3.Distance(transform.position, GameManager.singleton.listeJoueurs[i].position)) < distance)
            {
				joueurChoisi = GameManager.singleton.listeJoueurs[i];
				distance = Mathf.Abs(Vector3.Distance(transform.position, joueurChoisi.position));

			}
        }
		return joueurChoisi;
    }

}


