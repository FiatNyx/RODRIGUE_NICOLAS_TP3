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
	// Start is called before the first frame update
	void Start()
    {
		joueurMain = GetComponent<JoueurMain>();
		joueurAttaques = GetComponent<JoueurAttaques>();
	}

    // Update is called once per frame
    void Update()
    {

		if (GameManager.singleton.getPlayerTurn() && joueurMain.isAttacking == false)
        {

			joueurAttaques.AttaqueUpdate();

			//--------------------------------
			//Section des attaques. Le getTimerJoueur() est pour s'assurer que le joueur ait assez de temps pour payer l'attaque
			//---------------------------------

			if(joueurMain.moveSelected != 0)
            {
				if (Input.GetMouseButtonDown(0))
				{
					if (joueurMain.moveSelected == 1 && GameManager.singleton.getTimerJoueur() > 2)
					{
						joueurMain.isAttacking = true;
						joueurMain.animationJoueur.SetTrigger("FireballAttack");
						StartCoroutine(BouleDeFeu());

					}
					else if (joueurMain.moveSelected == 2 && GameManager.singleton.getTimerJoueur() > 4)
					{

						RaycastHit hit;
						if (Physics.Raycast(joueurMain.camRay, out hit))
						{
							GameManager.singleton.StartAttack(4);
							GameObject cercleLent = Instantiate(cercleLentPrefab, hit.point, transform.rotation);

							GameManager.singleton.FinishAttack();
						}
					}
					else if (joueurMain.moveSelected == 3 && GameManager.singleton.getTimerJoueur() > 3)
					{
						joueurMain.isAttacking = true;
						joueurMain.animationJoueur.SetTrigger("LightningAttack");
						StartCoroutine(Eclair());
						joueurMain.audioSource.PlayOneShot(audioEclair);
					}
					else if (joueurMain.moveSelected == 4 && GameManager.singleton.getTimerJoueur() > 6)
					{
						RaycastHit hit;
						if (Physics.Raycast(joueurMain.camRay, out hit))
						{
							GameManager.singleton.StartAttack(6);
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
        else
        {
			joueurAttaques.resetAttackSelected();
        }

	}

	/// <summary>
	/// S'occupe du lancement de l'attaque de boule de feu
	/// </summary>
	/// <returns></returns>
	IEnumerator BouleDeFeu()
	{
		GameManager.singleton.StartAttack(2);

		float timerMove = 0;
		while (joueurMain.isAttacking && timerMove < 1.5f)
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
		GameManager.singleton.StartAttack(3);

		while (joueurMain.isAttacking && timerMove < 1)
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
