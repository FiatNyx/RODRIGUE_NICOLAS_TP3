﻿using System.Collections;
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
    public float puissanceSlow = 0f;

    public Transform projectileStartPoint;

    [HideInInspector]
    public Animator animationJoueur;
    Rigidbody[] ragdollRBs;
    Collider[] ragdollColliders;
    public bool healedThisTurn = false;
    public Collider joueurCollider;

    public bool isDead = false;

    JoueurAttaques joueurAttaques;
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

    public void updateEffets()
    {
        if (isHealing)
        {
            heal(healStrength);
            healedThisTurn = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isDead == false)
        {
            camRay = mainCam.ScreenPointToRay(Input.mousePosition);
        }

    }

    public void heal(int healAmount)
    {
        if (isDead == false)
        {
            vie += healAmount;
            if (vie > vieMax)
            {
                vie = vieMax;
            }


            UI_Manager.singleton.changeVieText();
        }

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
            
            //Afin d'éviter des bugs
            StopAllCoroutines();



            // Scene scene = SceneManager.GetActiveScene();
            // SceneManager.LoadScene(scene.name);

            joueurAttaques.resetAttackSelected();

            //Audio de la mort
            audioSource.Stop();
            // audioSource.PlayOneShot(audioMortEnnemy);

            //Désactive tout
            animationJoueur.enabled = false;
            
            joueurCollider.enabled = false;
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

            //Continue à désactiver
            isDead = true;
        }
    }

    /// <summary>
	/// Détecte si le personnage est dans une zone de poison/ralentissement
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
    {
        if(isDead == false)
        {
            if(other.GetComponent<zoneLente>() != null)
            {
                isSlowed = true;
                puissanceSlow = other.GetComponent<zoneLente>().getSlowStrength();
            }

            if(other.GetComponent<zonePoison>() != null)
            {
                isPoisoned = true;
                puissancePoison = other.GetComponent<zonePoison>().getPoisonStrength();
            }
            if (other.tag == "attaqueEnnemy")
            {
                damage(other.GetComponent<Attaque>().damage);
                animationJoueur.SetTrigger("Hurt");
            }
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
            if (other.GetComponent<zoneLente>() != null)
            {
                isSlowed = false;
                puissanceSlow = other.GetComponent<zoneLente>().getSlowStrength();
            }

            if (other.GetComponent<zonePoison>() != null)
            {
                isPoisoned = false;
                puissancePoison = other.GetComponent<zonePoison>().getPoisonStrength();
            }

            if (other.GetComponent<zoneHeal>() != null)
            {
               
                isHealing = false;
            }
        }
    }
}