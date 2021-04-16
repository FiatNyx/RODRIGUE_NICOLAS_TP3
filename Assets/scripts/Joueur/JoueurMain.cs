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

    [HideInInspector]
    public float timerPoison = 0;

    [HideInInspector]
    public int vie = 30;

    [HideInInspector]
    public bool isThisPlayersTurn = false;

    
    public int vieMax = 30;

    public AudioSource audioSource;

    
    public AudioClip audioDamage;

    [HideInInspector]
    public bool isAttacking = false;

    [HideInInspector]
    public bool isSlowed = false;


    public Transform projectileStartPoint;

    [HideInInspector]
    public Animator animationJoueur;

    /// <summary>
	/// Initialisation de la variable de caméra et changement du texte de vie dans l'UI pour la vie du personnage
	/// </summary>
    void Start()
    {
        mainCam = Camera.main;
        UI_Manager.singleton.changeVieText();

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


    // Update is called once per frame
    void Update()
    {
        Debug.Log(isThisPlayersTurn);
        camRay = mainCam.ScreenPointToRay(Input.mousePosition);



    }

    /// <summary>
	/// Endommage le personnage et reload la scène s'il est tué
	/// </summary>
	/// <param name="damage"></param>
	public void damage(int damage)
    {
        vie -= damage;

        UI_Manager.singleton.changeVieText();
        audioSource.PlayOneShot(audioDamage);

        if (vie <= 0)
        {
            GameManager.singleton.killJoueur(transform);
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
            // Scene scene = SceneManager.GetActiveScene();
            // SceneManager.LoadScene(scene.name);
        }
    }

    /// <summary>
	/// Détecte si le personnage est dans une zone de poison/ralentissement
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "LentPoison")
        {
            isSlowed = true;
            isPoisoned = true;
        }
        if (other.tag == "attaqueEnnemy")
        {
            damage(other.GetComponent<Attaque>().damage);
            animationJoueur.SetTrigger("Hurt");
        }
    }

    /// <summary>
    /// Détecte si le personnage est sortit d'une zone de poison/ralentissement
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LentPoison")
        {
            isSlowed = false;
            isPoisoned = false;
        }

    }

}
