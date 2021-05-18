using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joueur2 : MonoBehaviour
{

	JoueurMain joueurMain;
	JoueurAttaques joueurAttaques;

	public GameObject cercleLentPrefab;
	public GameObject cerclePoisonPrefab;
	public GameObject cercleHealPrefab;
	public GameObject prefabCercleExplosion;

	int[] listeTypesAttaque;
	GameObject cercleLent;
	GameObject cerclePoison;
	GameObject cercleHeal;

	

	// Start is called before the first frame update
	void Start()
	{
		joueurMain = GetComponent<JoueurMain>();
		joueurAttaques = GetComponent<JoueurAttaques>();

		//Pour les types de marqueurs
		listeTypesAttaque = new int[4];
		listeTypesAttaque[0] = 2;
		listeTypesAttaque[1] = 2;
		listeTypesAttaque[2] = 2;
		listeTypesAttaque[3] = 3;

		//Instancie les 3 cercles de malédiction dans un endroit inaccessible
		cercleLent = Instantiate(cercleLentPrefab, new Vector3(0, -100, 0), transform.rotation);
		cerclePoison = Instantiate(cerclePoisonPrefab, new Vector3(0, -100, 0), transform.rotation);
		cercleHeal = Instantiate(cercleHealPrefab, new Vector3(0, -100, 0), transform.rotation);

		//Stocke les 3 cercles de malédiction
		GameManager.singleton.listeObjectsTemporaires.Add(cercleLent.GetComponent<ObjectTemporaire>());
		GameManager.singleton.listeObjectsTemporaires.Add(cerclePoison.GetComponent<ObjectTemporaire>());
		GameManager.singleton.listeObjectsTemporaires.Add(cercleHeal.GetComponent<ObjectTemporaire>());

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
		if (joueurMain.isDead == false && GameManager.singleton.isPaused == false)
		{
			if (joueurMain.isThisPlayersTurn && joueurMain.isAttacking == false)
			{
				//Crée une liste pour le ciblage de l'attaque 4
				List<GameObject> liste = new List<GameObject>
				{
					cercleHeal,
					cercleLent,
					cerclePoison
				};
				//Call "L'update" de "JoueurAttaque"
				joueurAttaques.AttaqueUpdate(listeTypesAttaque, liste);

				//--------------------------------
				//Section des attaques. Le getTimerJoueur() est pour s'assurer que le joueur ait assez de temps pour payer l'attaque
				//---------------------------------
				
				if (joueurMain.moveSelected != 0)
				{
					if (Input.GetMouseButtonDown(0))
					{
						if (joueurMain.moveSelected == 1 && GameManager.singleton.getTimerJoueur() > 3 * joueurMain.puissanceSlow)
						{
							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit, 100, joueurMain.teleportLayer))
							{
								joueurAttaques.resetAttackSelected();
								joueurMain.isAttacking = true;
								joueurMain.animationJoueur.SetTrigger("areaAttack");
								StartCoroutine(CurseSlow(hit));
							}
							

						}
						else if (joueurMain.moveSelected == 2 && GameManager.singleton.getTimerJoueur() > 3 * joueurMain.puissanceSlow)
						{
							RaycastHit hit;

							if (Physics.Raycast(joueurMain.camRay, out hit, 100, joueurMain.teleportLayer))
							{
								joueurAttaques.resetAttackSelected();
								joueurMain.isAttacking = true;
								joueurMain.animationJoueur.SetTrigger("areaAttack");
								StartCoroutine(CursePoison(hit));
							}


						}
						else if (joueurMain.moveSelected == 3 && GameManager.singleton.getTimerJoueur() > 2 * joueurMain.puissanceSlow)
						{
							RaycastHit hit;
							if (Physics.Raycast(joueurMain.camRay, out hit, 100, joueurMain.teleportLayer))
							{
								joueurAttaques.resetAttackSelected();
								joueurMain.isAttacking = true;
								joueurMain.animationJoueur.SetTrigger("areaAttack");
								StartCoroutine(HealCircle(hit));
							}


						}
						else if(joueurMain.moveSelected == 4 && GameManager.singleton.getTimerJoueur() > 6 * joueurMain.puissanceSlow)
						{
							joueurAttaques.resetAttackSelected();
							joueurMain.isAttacking = true;
							joueurMain.animationJoueur.SetTrigger("areaAttack");
							StartCoroutine(ExplosionCircle());
						}
						
					}
				}

			}
			
		}


	}

	/// <summary>
	/// S'occupe du lancement de l'attaque du cercle de ralentissement
	/// </summary>
	/// <returns></returns>
	IEnumerator CurseSlow(RaycastHit hit)
	{

		
		GameManager.singleton.StartAttack(3 * joueurMain.puissanceSlow);
		cercleLent.GetComponent<SphereCollider>().enabled = false;
		GameManager.singleton.ResetSlow();
		yield return new WaitForSeconds(1.75f * joueurMain.puissanceSlow);

		GameManager.singleton.changeLevelSlow(1);
		cercleLent.SetActive(true);
		cercleLent.GetComponent<zoneLente>().UpdateParticules();

		cercleLent.transform.position = hit.point;
		cercleLent.transform.rotation = transform.rotation;
		cercleLent.GetComponent<SphereCollider>().enabled = true;

		yield return new WaitForSeconds(1f * joueurMain.puissanceSlow);

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
		
	}

	/// <summary>
	/// S'occupe du lancement de l'attaque du cercle de poison
	/// </summary>
	/// <returns></returns>
	IEnumerator CursePoison(RaycastHit hit)
	{


		GameManager.singleton.StartAttack(3 * joueurMain.puissanceSlow);
		cerclePoison.GetComponent<SphereCollider>().enabled = false;
		GameManager.singleton.ResetPoison();

		yield return new WaitForSeconds(1.75f * joueurMain.puissanceSlow);

		GameManager.singleton.changeLevelPoison(1);

		cerclePoison.GetComponent<zonePoison>().UpdateParticules();
		cerclePoison.SetActive(true);
		cerclePoison.transform.position = hit.point;
		cerclePoison.transform.rotation = transform.rotation;
		cerclePoison.GetComponent<SphereCollider>().enabled = true;

		yield return new WaitForSeconds(1f * joueurMain.puissanceSlow);

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
		
	}

	/// <summary>
	/// S'occupe du lancement de l'attaque du cercle de soin
	/// </summary>
	/// <returns></returns>
	IEnumerator HealCircle(RaycastHit hit)
	{


		GameManager.singleton.StartAttack(2 * joueurMain.puissanceSlow);
		cercleHeal.GetComponent<SphereCollider>().enabled = false;

		yield return new WaitForSeconds(1.75f * joueurMain.puissanceSlow);

		GameManager.singleton.changeLevelHeal(1);
		cercleHeal.GetComponent<zoneHeal>().UpdateParticules();
		cercleHeal.SetActive(true);
		cercleHeal.transform.position = hit.point;
		cercleHeal.transform.rotation = transform.rotation;
		cercleHeal.GetComponent<SphereCollider>().enabled = true;

		yield return new WaitForSeconds(1f * joueurMain.puissanceSlow);

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();
		
	}

	/// <summary>
	/// S'occupe du lancement de l'attaque d'explosion des cercles
	/// </summary>
	/// <returns></returns>
	IEnumerator ExplosionCircle()
	{
		GameManager.singleton.StartAttack(6 * joueurMain.puissanceSlow);
		cercleHeal.GetComponent<SphereCollider>().enabled = false;

		yield return new WaitForSeconds(1.75f * joueurMain.puissanceSlow);

		//Si le cercle de slow a été lancé, l'explose
		if(GameManager.singleton.levelSlow > 0)
		{
			GameObject explosion = Instantiate(prefabCercleExplosion, cercleLent.transform.position, cercleLent.transform.rotation);
			explosion.GetComponent<ExplosionCircle>().isDamage = true;

			switch (GameManager.singleton.levelSlow)
			{
				case 1:
					explosion.GetComponent<ExplosionCircle>().damageAmount = 10;
					break;
				case 2:
					explosion.GetComponent<ExplosionCircle>().damageAmount = 20;
					break;
				case 3:
					explosion.GetComponent<ExplosionCircle>().damageAmount = 30;
					break;
				default:
					break;
			}
		}

		//Si le cercle de poison a été lancé, l'explose
		if (GameManager.singleton.levelPoison > 0)
		{
			GameObject explosion = Instantiate(prefabCercleExplosion, cerclePoison.transform.position, cerclePoison.transform.rotation);
			explosion.GetComponent<ExplosionCircle>().isDamage = true;

			switch (GameManager.singleton.levelPoison)
			{
				case 1:
					explosion.GetComponent<ExplosionCircle>().damageAmount = 10;
					break;
				case 2:
					explosion.GetComponent<ExplosionCircle>().damageAmount = 20;
					break;
				case 3:
					explosion.GetComponent<ExplosionCircle>().damageAmount = 30;
					break;
				default:
					break;
			}
		}

		//Si le cercle de soin a été lancé, l'explose
		if (GameManager.singleton.levelHeal > 0)
		{
			GameObject explosion = Instantiate(prefabCercleExplosion, cercleHeal.transform.position, cercleHeal.transform.rotation);
			explosion.GetComponent<ExplosionCircle>().isDamage = false;

			switch (GameManager.singleton.levelHeal)
			{
				case 1:
					explosion.GetComponent<ExplosionCircle>().healAmount = 10;
					break;
				case 2:
					explosion.GetComponent<ExplosionCircle>().healAmount = 20;
					break;
				case 3:
					explosion.GetComponent<ExplosionCircle>().healAmount = 30;
					break;
				default:
					break;
			}
		}


		yield return new WaitForSeconds(1f * joueurMain.puissanceSlow);

		//Enlève tous les cercles
		GameManager.singleton.ResetPoison();
		GameManager.singleton.ResetSlow();
		GameManager.singleton.levelHeal = 0;
		GameManager.singleton.levelPoison = 0;
		GameManager.singleton.levelSlow = 0;
		cercleLent.GetComponent<ObjectTemporaire>().updateEffects = true;
		cercleHeal.GetComponent<ObjectTemporaire>().updateEffects = true;
		cerclePoison.GetComponent<ObjectTemporaire>().updateEffects = true;

		joueurMain.isAttacking = false;
		GameManager.singleton.FinishAttack();

	}
}
