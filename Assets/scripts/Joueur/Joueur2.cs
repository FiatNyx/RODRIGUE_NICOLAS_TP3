using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joueur2 : MonoBehaviour
{

	JoueurMain joueurMain;
	JoueurAttaques joueurAttaques;

	public GameObject cercleLentPrefab;
	int[] listeTypesAttaque;

	// Start is called before the first frame update
	void Start()
	{
		joueurMain = GetComponent<JoueurMain>();
		joueurAttaques = GetComponent<JoueurAttaques>();
		listeTypesAttaque = new int[4];
		listeTypesAttaque[0] = 1;
		listeTypesAttaque[1] = 1;
		listeTypesAttaque[2] = 1;
		listeTypesAttaque[3] = 0;
	}

	// Update is called once per frame
	void Update()
	{
		if (joueurMain.isDead == false)
		{
			if (joueurMain.isThisPlayersTurn && joueurMain.isAttacking == false)
			{

				joueurAttaques.AttaqueUpdate(listeTypesAttaque);

				//--------------------------------
				//Section des attaques. Le getTimerJoueur() est pour s'assurer que le joueur ait assez de temps pour payer l'attaque
				//---------------------------------
				
				if (joueurMain.moveSelected != 0)
				{
					if (Input.GetMouseButtonDown(0))
					{
						if (joueurMain.moveSelected == 1 && GameManager.singleton.getTimerJoueur() > 2)
						{
							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit))
							{
								joueurMain.isAttacking = true;
								joueurMain.animationJoueur.SetTrigger("areaAttack");
								StartCoroutine(CurseSlow(hit));
							}
							

						}
						/*else if (joueurMain.moveSelected == 2 && GameManager.singleton.getTimerJoueur() > 4)
						{

							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit))
							{
								GameManager.singleton.StartAttack(4);
								GameObject cercleLent = Instantiate(cercleLentPrefab, hit.point, transform.rotation);
								joueurAttaques.resetAttackSelected();
								GameManager.singleton.FinishAttack();
							}
						}
						else if (joueurMain.moveSelected == 3 && GameManager.singleton.getTimerJoueur() > 3)
						{
							joueurMain.isAttacking = true;
							joueurMain.animationJoueur.SetTrigger("areaAttack");
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


						}*/
					}
				}

			}
			else
			{
				joueurAttaques.resetAttackSelected();
			}
		}


	}

	/// <summary>
	/// S'occupe du lancement de l'attaque d'éclair
	/// </summary>
	/// <returns></returns>
	IEnumerator CurseSlow(RaycastHit hit)
	{

		
		GameManager.singleton.StartAttack(3);

		yield return new WaitForSeconds(1.5f);

		GameObject cercleLent = Instantiate(cercleLentPrefab, hit.point, transform.rotation);


		yield return new WaitForSeconds(1);

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
		joueurAttaques.resetAttackSelected();
	}

}
