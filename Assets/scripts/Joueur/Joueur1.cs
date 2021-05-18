using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joueur1 : MonoBehaviour
{

	JoueurMain joueurMain;
	JoueurAttaques joueurAttaques;
	public GameObject fireball;
	public GameObject eclair;


	public AudioClip audioEclair;
	public AudioClip audioZoom;
	public AudioClip audioBoom;
	public ParticleSystem teleportParticles;
	public GameObject explosionPrefab;
	public GameObject coneFeuPrefab;
	public GameObject murFeuPrefab;
	int[] listeTypesAttaque;


	// Start is called before the first frame update
	void Start()
    {
		joueurMain = GetComponent<JoueurMain>();
		joueurAttaques = GetComponent<JoueurAttaques>();

		//Pour les types de marqueurs
		listeTypesAttaque = new int[4];
		listeTypesAttaque[0] = 0;
		listeTypesAttaque[1] = 0;
		listeTypesAttaque[2] = 2;
		listeTypesAttaque[3] = 2;

		//Change la vie du joueur selon la difficulté
		switch (DataManager.singleton.difficulte)
		{
			case "Facile":
				joueurMain.vieMax = 40;
				break;
			case "Normal":
				joueurMain.vieMax = 30;
				break;
			case "Difficile":
				joueurMain.vieMax = 20;
				break;
			default:
				break;
		}
		joueurMain.vie = joueurMain.vieMax;
	}

    // Update is called once per frame
    void Update()
    {
		//Vérifie pour éviter les bugs
		if(joueurMain.isDead == false && GameManager.singleton.isPaused == false)
        {
			if (joueurMain.isThisPlayersTurn && joueurMain.isAttacking == false)
			{
				//Call "L'update" de "JoueurAttaque"
				joueurAttaques.AttaqueUpdate(listeTypesAttaque, null);

				//--------------------------------
				//Section des attaques. Le getTimerJoueur() est pour s'assurer que le joueur ait assez de temps pour payer l'attaque
				//---------------------------------

				if (joueurMain.moveSelected != 0)
				{
					if (Input.GetMouseButtonDown(0))
					{
						if (joueurMain.moveSelected == 1 && GameManager.singleton.getTimerJoueur() > 2 * joueurMain.puissanceSlow)
						{
							joueurAttaques.resetAttackSelected();
							joueurMain.isAttacking = true;
							joueurMain.animationJoueur.SetTrigger("FireballAttack");
							StartCoroutine(BouleDeFeu());

						}
						else if (joueurMain.moveSelected == 2 && GameManager.singleton.getTimerJoueur() > 4 * joueurMain.puissanceSlow)
						{
							
							joueurAttaques.resetAttackSelected();
							joueurMain.isAttacking = true;
							joueurMain.animationJoueur.SetTrigger("GroundAttack");
							StartCoroutine(ConeDeFeu());
							
						}
						else if (joueurMain.moveSelected == 3 && GameManager.singleton.getTimerJoueur() > 3 * joueurMain.puissanceSlow)
						{
							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit, 500, joueurMain.teleportLayer))
							{
								joueurAttaques.resetAttackSelected();
								joueurMain.isAttacking = true;
								joueurMain.animationJoueur.SetTrigger("GroundAttack");
								StartCoroutine(MurDeFeu(hit));
							}
						}
						else if (joueurMain.moveSelected == 4 && GameManager.singleton.getTimerJoueur() > 0.2 * joueurMain.puissanceSlow)
						{
							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit, 500, joueurMain.teleportLayer))
							{
								joueurAttaques.resetAttackSelected();
								int puissance = Mathf.RoundToInt(GameManager.singleton.getTimerJoueur() * 3);
								GameManager.singleton.StartAttack(GameManager.singleton.getTimerJoueur() - 0.2f);
								transform.position = hit.point;
								
								GameObject explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
								explosion.GetComponent<Attaque>().damage = puissance;
								joueurMain.Enflammer(1);
								GameManager.singleton.FinishAttack();
								
								joueurMain.audioSource.PlayOneShot(audioZoom);
							}


						}
					}
				}

			}
			
		}
		

	}

	/// <summary>
	/// S'occupe du lancement de l'attaque de boule de feu
	/// </summary>
	/// <returns></returns>
	IEnumerator BouleDeFeu()
	{
		GameManager.singleton.StartAttack(2 * joueurMain.puissanceSlow);

		yield return new WaitForSeconds(1.5f * joueurMain.puissanceSlow);



		GameObject bouleDeFeu = Instantiate(fireball, joueurMain.projectileStartPoint.position, joueurMain.projectileStartPoint.rotation);
		bouleDeFeu.GetComponent<Fireball>().joueur = joueurMain;

		yield return new WaitForSeconds(2f * joueurMain.puissanceSlow);

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
	
	}

	/// <summary>
	/// S'occupe du lancement de l'attaque de cone de feu
	/// </summary>
	/// <returns></returns>
	IEnumerator ConeDeFeu()
	{
		GameManager.singleton.StartAttack(2 * joueurMain.puissanceSlow);

		yield return new WaitForSeconds(1.25f * joueurMain.puissanceSlow);

		GameObject coneFeu = Instantiate(coneFeuPrefab, joueurMain.projectileStartPoint.position, joueurMain.projectileStartPoint.rotation);

		yield return new WaitForSeconds(1.5f * joueurMain.puissanceSlow);

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
	}

	/// <summary>
	/// S'occupe du lancement de l'attaque du mur de feu
	/// </summary>
	/// <returns></returns>
	IEnumerator MurDeFeu(RaycastHit hit)
	{
		GameManager.singleton.StartAttack(2 * joueurMain.puissanceSlow);

		yield return new WaitForSeconds(1.25f * joueurMain.puissanceSlow);

		GameObject murDeFeu = Instantiate(murFeuPrefab, hit.point, joueurMain.transform.rotation);
		GameManager.singleton.listeObjectsTemporaires.Add(murDeFeu.GetComponent<ObjectTemporaire>());
		yield return new WaitForSeconds(1f * joueurMain.puissanceSlow);

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
	}

}
