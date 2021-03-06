using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class JoueurMain : MonoBehaviour
{

    Camera mainCam;

    [HideInInspector]
    public Ray camRay;

    [HideInInspector]
    public int moveSelected = 0;

    [HideInInspector]
    public bool isPoisoned = false;
    public int puissancePoison = 0;

    [HideInInspector]
    public float timerPoison = 0;

    [HideInInspector]
    public int vie = 30;

    public LayerMask teleportLayer;

    [HideInInspector]
    public bool isThisPlayersTurn = false;

    public bool isHealing = false;

    public int vieMax = 30;

    public AudioSource audioSource;

    public int healStrength = 0;
    public AudioClip audioDamage;

    [HideInInspector]
    public bool isAttacking = false;

    [HideInInspector]
    public bool isSlowed = false;
    public float puissanceSlow = 1f;

    public Transform projectileStartPoint;

    [HideInInspector]
    public Animator animationJoueur;
    Rigidbody[] ragdollRBs;
    Collider[] ragdollColliders;
    public bool healedThisTurn = false;
    public Collider joueurCollider;

    public int burnStatus = 0;
    public bool isDead = false;
	public GameObject particulesBrulure;
	public GameObject HealParticules;
    JoueurAttaques joueurAttaques;
	Rigidbody playerRigidbody;

    /// <summary>
	/// Initialisation de la variable de caméra et changement du texte de vie dans l'UI pour la vie du personnage
	/// </summary>
    void Start()
    {
        mainCam = Camera.main;
        UI_Manager.singleton.changeVieText();
        ragdollRBs = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();
        joueurAttaques = GetComponent<JoueurAttaques>();
		playerRigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
	/// Stock certains component dans des variables et initialise la vie selon la vie maximale
	/// </summary>
    private void Awake()
    {
        vie = vieMax;
        audioSource = GetComponent<AudioSource>();
        animationJoueur = GetComponent<Animator>();

    }

	/// <summary>
	/// Applique les effets de status au début du tour
	/// </summary>
    public void updateEffets()
    {
        if (isHealing)
        {
            heal(healStrength);
            healedThisTurn = true;
        }

        if(burnStatus > 0)
        {
            int previousBurn = burnStatus;
            burnStatus = (int)Mathf.Floor(burnStatus / 2f);

            damage((previousBurn - burnStatus) * 10);
			if(burnStatus <= 0)
			{
				particulesBrulure.SetActive(false);
				burnStatus = 0;
			}
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == false && GameManager.singleton.isPaused == false)
        {
            if(isSlowed == false)
            {
                puissanceSlow = 1;
            }
            camRay = mainCam.ScreenPointToRay(Input.mousePosition);
  

        }

    }

	/// <summary>
	/// Soigne le personnage
	/// </summary>
	/// <param name="healAmount">Le montant à soigner</param>
    public void heal(int healAmount)
    {
        if (isDead == false)
        {
			HealParticules.SetActive(true);
            vie += healAmount;
			StartCoroutine(DeleteParticulesHeal());
            if (vie > vieMax)
            {
                vie = vieMax;
            }


            UI_Manager.singleton.changeVieText();
        }

    }

	/// <summary>
	/// Un timer qui désactive les particules qui sont activées par un soin.
	/// </summary>
	/// <returns></returns>
	IEnumerator DeleteParticulesHeal()
	{
		yield return new WaitForSeconds(2f);

		HealParticules.SetActive(false);
	}

    /// <summary>
	/// Endommage le personnage et s'occupe de tuer le personnage
	/// </summary>
	/// <param name="damage"></param>
	public void damage(int damage)
    {
        vie -= damage;
		GameManager.singleton.nbDegatTotal += damage;
        
        audioSource.PlayOneShot(audioDamage);

		//S'il est mort
        if (vie <= 0)
        {
			vie = 0;
			GameManager.singleton.timerJoueur = 0.1f;
            
            //Afin d'éviter des bugs
            StopAllCoroutines();

            joueurAttaques.resetAttackSelected();

            //Audio désactiver l'audio source
            audioSource.Stop();

            //Désactive tout
            animationJoueur.enabled = false;
            
            //L'enlève de la liste
            GameManager.singleton.killJoueur(transform);

            //Active le ragdoll
            foreach (Collider rbcollider in ragdollColliders)
            {
                rbcollider.enabled = true;
            }

            foreach (Rigidbody rb in ragdollRBs)
            {
                rb.isKinematic = false;
            }

			joueurCollider.enabled = false;
			playerRigidbody.isKinematic = true;
			//Continue à désactiver
			isDead = true;
        }

		//Update l'UI
		UI_Manager.singleton.changeVieText();
	}

	/// <summary>
	/// Inflige une brulure au personnage
	/// </summary>
	/// <param name="burnAmount">Le nombre de brulures à infliger</param>
    public void Enflammer(int burnAmount)
    {
		particulesBrulure.SetActive(true);
        burnStatus += burnAmount;

    }

    /// <summary>
	/// Détecte si le personnage est dans une zone de poison/ralentissement
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
    {
        if(isDead == false)
        {
			//Zone de ralentissement
            if(other.GetComponent<zoneLente>() != null)
            {
                isSlowed = true;
                puissanceSlow = other.GetComponent<zoneLente>().getSlowStrength();
            }

			//Zone de poison
            if(other.GetComponent<zonePoison>() != null)
            {
                isPoisoned = true;
                puissancePoison = other.GetComponent<zonePoison>().getPoisonStrength();
            }

			//Attaque ennemie
            if (other.tag == "attaqueEnnemy")
            {
                damage(other.GetComponent<Attaque>().damage);
                animationJoueur.SetTrigger("Hurt");
            }

			//Zone de soin
            if(other.GetComponent<zoneHeal>() != null)
            {
                healStrength = other.GetComponent<zoneHeal>().getHealStrength();

                if(healedThisTurn == false)
                {
                    heal(healStrength);
                    healedThisTurn = true;
                }
               
                isHealing = true;
            }

			//Explosion
			if(other.GetComponent<ExplosionCircle>() != null)
			{
				if(other.GetComponent<ExplosionCircle>().isDamage == false)
				{
					heal(other.GetComponent<ExplosionCircle>().healAmount);
				}
			}

			//Attaque qui enflamme
            if (other.GetComponent<FeuStatusAttack>() != null)
            {
                Enflammer(other.GetComponent<FeuStatusAttack>().burnAmount);
            }
        }
        
    }

    /// <summary>
    /// Détecte si le personnage est sortit d'une zone de poison/ralentissement
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if(isDead == false)
        {
			//Zone de ralentissement
            if (other.GetComponent<zoneLente>() != null)
            {
                isSlowed = false;
                puissanceSlow = other.GetComponent<zoneLente>().getSlowStrength();
            }

			//Zone de poison
            if (other.GetComponent<zonePoison>() != null)
            {
                isPoisoned = false;
                puissancePoison = other.GetComponent<zonePoison>().getPoisonStrength();
            }

			//Zone de heal
            if (other.GetComponent<zoneHeal>() != null)
            {
                isHealing = false;
            }
        }
    }
}
