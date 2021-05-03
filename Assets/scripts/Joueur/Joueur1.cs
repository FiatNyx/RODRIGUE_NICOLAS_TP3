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
	public GameObject cercleLentPrefab;
	
	int[] listeTypesAttaque;
	// Start is called before the first frame update
	void Start()
    {
		joueurMain = GetComponent<JoueurMain>();
		joueurAttaques = GetComponent<JoueurAttaques>();
		listeTypesAttaque = new int[4];
		listeTypesAttaque[0] = 0;
		listeTypesAttaque[1] = 2;
		listeTypesAttaque[2] = 0;
		listeTypesAttaque[3] = 2;
	}

    // Update is called once per frame
    void Update()
    {
		if(joueurMain.isDead == false)
        {
			if (joueurMain.isThisPlayersTurn && joueurMain.isAttacking == false)
			{

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

							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit, 500, joueurMain.teleportLayer))
							{
								joueurAttaques.resetAttackSelected();
								GameManager.singleton.StartAttack(4 * joueurMain.puissanceSlow);
								GameObject cercleLent = Instantiate(cercleLentPrefab, hit.point, transform.rotation);
								
								GameManager.singleton.FinishAttack();
							}
						}
						else if (joueurMain.moveSelected == 3 && GameManager.singleton.getTimerJoueur() > 3 * joueurMain.puissanceSlow)
						{
							joueurAttaques.resetAttackSelected();
							joueurMain.isAttacking = true;
							joueurMain.animationJoueur.SetTrigger("LightningAttack");
							StartCoroutine(Eclair());
							joueurMain.audioSource.PlayOneShot(audioEclair);
						}
						else if (joueurMain.moveSelected == 4 && GameManager.singleton.getTimerJoueur() > 6 * joueurMain.puissanceSlow)
						{
							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit, 500, joueurMain.teleportLayer))
							{
								joueurAttaques.resetAttackSelected();
								GameManager.singleton.StartAttack(6 * joueurMain.puissanceSlow);
								transform.position = hit.point;
								teleportParticles.Play();
								teleportParticles.GetComponent<TeleportParticles>().Spin();
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

		float timerMove = 0;
		while (joueurMain.isAttacking && timerMove < 1.5f * joueurMain.puissanceSlow)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}



		GameObject bouleDeFeu = Instantiate(fireball, joueurMain.projectileStartPoint.position, joueurMain.projectileStartPoint.rotation);
		bouleDeFeu.GetComponent<Fireball>().joueur = joueurMain;

		timerMove = 0;
		while (joueurMain.isAttacking && timerMove < 2)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
	
	}

	/// <summary>
	/// S'occupe du lancement de l'attaque d'éclair
	/// </summary>
	/// <returns></returns>
	IEnumerator Eclair()
	{

		float timerMove = 0;
		GameManager.singleton.StartAttack(3 * joueurMain.puissanceSlow);

		while (joueurMain.isAttacking && timerMove < 1 * joueurMain.puissanceSlow)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}
		GameObject eclairInstance = Instantiate(eclair, joueurMain.projectileStartPoint.position, joueurMain.projectileStartPoint.rotation);
		eclairInstance.GetComponent<Eclair>().joueur = joueurMain;

		timerMove = 0;
		while (joueurMain.isAttacking && timerMove < 2)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
		
	}

}
