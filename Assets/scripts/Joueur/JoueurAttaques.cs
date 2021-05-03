using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurAttaques : MonoBehaviour
{
    JoueurMain joueurMain;

    public GameObject marqueur1;
    public GameObject marqueur2;
    public GameObject marqueur3;
    public GameObject marqueur4;

	GameObject[] listeMarqueurs;

	List<GameObject> listeMarqueursInstancies;
	// Start is called before the first frame update
	void Start()
    {
        joueurMain = GetComponent<JoueurMain>();
		listeMarqueurs = new GameObject[4];
		listeMarqueurs[0] = marqueur1;
		listeMarqueurs[1] = marqueur2;
		listeMarqueurs[2] = marqueur3;
		listeMarqueurs[3] = marqueur4;
		listeMarqueursInstancies = new List<GameObject>();
	}

	/// <summary>
	/// Ce qui gère les marqueurs d'attaques et la sélection d'attaque
	/// </summary>
	/// <param name="attaques">Les types des attaques 1 (0 = ligne droite, 1 = zone ciblée avec la souris, 2 = zone ciblée avec la souris sans teleport)
	/// 3 = zone fixe sur points spécifiques</param>
	public void AttaqueUpdate(int[] attaques, List<GameObject> listeCiblesMarqueur)
	{


		//Clic droit pour annuler une attaque
		if (Input.GetMouseButtonDown(1))
		{
			resetAttackSelected();
		}



		//--------------------
		//Sélection des attaques     \\\\\\!@\!@\@\   À CHANGER POUR S'ADAPTER AU TYPE DE MARQUEUR
		//-------------------
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			effacerMarqueurs();
			marqueur1.SetActive(true);
			joueurMain.moveSelected = 1;
			UI_Manager.singleton.changeSelectedMove(1);
			
			if (attaques[joueurMain.moveSelected - 1] == 3)
			{
				for (int i = 0; i < listeCiblesMarqueur.Count; i++)
				{
					GameObject marqueurInstancie = Instantiate(marqueur1, listeCiblesMarqueur[i].transform.position, listeCiblesMarqueur[i].transform.rotation);
					listeMarqueursInstancies.Add(marqueurInstancie);
				}
			}
			else
			{
				marqueur1.SetActive(true);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			effacerMarqueurs();
			marqueur2.SetActive(true);
			joueurMain.moveSelected = 2;
			UI_Manager.singleton.changeSelectedMove(2);
			if (attaques[joueurMain.moveSelected - 1] == 3)
			{
				for (int i = 0; i < listeCiblesMarqueur.Count; i++)
				{
					GameObject marqueurInstancie = Instantiate(marqueur2, listeCiblesMarqueur[i].transform.position, listeCiblesMarqueur[i].transform.rotation);
					listeMarqueursInstancies.Add(marqueurInstancie);
				}
			}
			else
			{
				marqueur2.SetActive(true);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			effacerMarqueurs();
			marqueur3.SetActive(true);
			joueurMain.moveSelected = 3;
			UI_Manager.singleton.changeSelectedMove(3);
			if (attaques[joueurMain.moveSelected - 1] == 3)
			{
				for (int i = 0; i < listeCiblesMarqueur.Count; i++)
				{
					GameObject marqueurInstancie = Instantiate(marqueur3, listeCiblesMarqueur[i].transform.position, listeCiblesMarqueur[i].transform.rotation);
					listeMarqueursInstancies.Add(marqueurInstancie);
				}
			}
			else
			{
				marqueur3.SetActive(true);
			}
		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			effacerMarqueurs();
			
			joueurMain.moveSelected = 4;
			UI_Manager.singleton.changeSelectedMove(4);
			if (attaques[joueurMain.moveSelected - 1] == 3)
			{
				for (int i = 0; i < listeCiblesMarqueur.Count; i++)
				{
					GameObject marqueurInstancie = Instantiate(marqueur4, listeCiblesMarqueur[i].transform.position, listeCiblesMarqueur[i].transform.rotation);
					marqueurInstancie.SetActive(true);
					listeMarqueursInstancies.Add(marqueurInstancie);
				}
			}
			else
			{
				marqueur4.SetActive(true);
			}
		}


		if (joueurMain.moveSelected != 0)
		{
			RaycastHit hit;
			if (attaques[joueurMain.moveSelected - 1] != 2)
			{
				if (Physics.Raycast(joueurMain.camRay, out hit))
				{
					if (attaques[joueurMain.moveSelected - 1] == 0)
					{
						listeMarqueurs[joueurMain.moveSelected - 1].transform.position = transform.position;
						joueurMain.projectileStartPoint.LookAt(hit.point);
						listeMarqueurs[joueurMain.moveSelected - 1].transform.LookAt(hit.point);
					}
					else if (attaques[joueurMain.moveSelected - 1] == 1)
					{
						listeMarqueurs[joueurMain.moveSelected - 1].transform.position = hit.point;
					}

				}

				
			}
            else
            {
				if (Physics.Raycast(joueurMain.camRay, out hit, 200, joueurMain.teleportLayer))
				{
					listeMarqueurs[joueurMain.moveSelected - 1].transform.position = hit.point;
				}

			}
		}
	}

		
        







    /// <summary>
	/// Efface tous les marqueurs et remet l'attaque sélectionnée à rien
	/// </summary>
	public void resetAttackSelected()
    {
        joueurMain.moveSelected = 0;
        effacerMarqueurs();
        UI_Manager.singleton.changeSelectedMove(0);
    }

    /// <summary>
	/// Efface les marqueurs
	/// </summary>
	private void effacerMarqueurs()
    {
        marqueur1.SetActive(false);
        marqueur2.SetActive(false);
        marqueur3.SetActive(false);
        marqueur4.SetActive(false);

		while (listeMarqueursInstancies.Count > 0)
		{
			Destroy(listeMarqueursInstancies[0]);
			listeMarqueursInstancies.RemoveAt(0);
		}
    }

}
