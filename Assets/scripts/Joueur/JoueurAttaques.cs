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
	
	// Start is called before the first frame update
	void Start()
    {
        joueurMain = GetComponent<JoueurMain>();
		listeMarqueurs = new GameObject[4];
		listeMarqueurs[0] = marqueur1;
		listeMarqueurs[1] = marqueur2;
		listeMarqueurs[2] = marqueur3;
		listeMarqueurs[3] = marqueur4;
    }

	/// <summary>
	/// Ce qui gère les marqueurs d'attaques et la sélection d'attaque
	/// </summary>
	/// <param name="attaques">Les types des attaques 1 (0 = ligne droite, 1 = zone ciblée avec la souris, 2 = zone ciblée avec la souris sans teleport)</param>
	public void AttaqueUpdate(int[] attaques)
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

		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			effacerMarqueurs();
			marqueur2.SetActive(true);
			joueurMain.moveSelected = 2;
			UI_Manager.singleton.changeSelectedMove(2);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			effacerMarqueurs();
			marqueur3.SetActive(true);
			joueurMain.moveSelected = 3;
			UI_Manager.singleton.changeSelectedMove(3);

		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			effacerMarqueurs();
			marqueur4.SetActive(true);
			joueurMain.moveSelected = 4;
			UI_Manager.singleton.changeSelectedMove(4);

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
    }

}
