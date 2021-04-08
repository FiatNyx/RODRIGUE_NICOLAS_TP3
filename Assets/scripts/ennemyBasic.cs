using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ennemyBasic : MonoBehaviour
{
	NavMeshAgent navMeshAgent;

	public bool isMoving = false;
	public GameObject player;

	public int maxHealth = 50;
	int health = 50;
	public GameObject sliderVie;


	private Animator animationEnnemy;


	public bool isPoisoned;
	public float timerPoison = 0f;
	float speed = 0f;
	public bool isThisEnnemyTurn;
	

	AudioSource audioSource;
	public AudioClip audioOuch;
	
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


