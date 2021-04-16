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



	// Start is called before the first frame update
	void Start()
    {
        joueurMain = GetComponent<JoueurMain>();
    }

    // Update is called once per frame
    public void AttaqueUpdate()
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
			marqueur1.transform.position = transform.position;
			joueurMain.moveSelected = 1;
			UI_Manager.singleton.changeSelectedMove(1);

		}
		else if (Input.GetKeyDown(KeyCode.Alpha2))
		{
			effacerMarqueurs();
	


			RaycastHit hit;
			if (Physics.Raycast(joueurMain.camRay, out hit))
			{
				marqueur2.SetActive(true);
				marqueur2.transform.position = hit.point;

			}
			joueurMain.moveSelected = 2;
			UI_Manager.singleton.changeSelectedMove(2);
		}
		else if (Input.GetKeyDown(KeyCode.Alpha3))
		{
			effacerMarqueurs();
			marqueur3.SetActive(true);
			marqueur3.transform.position = transform.position;
			joueurMain.moveSelected = 3;
			UI_Manager.singleton.changeSelectedMove(3);

		}
		else if (Input.GetKeyDown(KeyCode.Alpha4))
		{
			effacerMarqueurs();
			


			RaycastHit hit;
			if (Physics.Raycast(joueurMain.camRay, out hit))
			{
				marqueur4.SetActive(true);
				marqueur4.transform.position = hit.point;

			}

			joueurMain.moveSelected = 4;
			UI_Manager.singleton.changeSelectedMove(4);

		}


		if(joueurMain.moveSelected != 0)
        {
			RaycastHit hit;
			if (Physics.Raycast(joueurMain.camRay, out hit))
			{
				//S'occupe de la rotation de tous les marqueurs. Ainsi que leur position
				joueurMain.projectileStartPoint.LookAt(hit.point);
				marqueur1.transform.LookAt(hit.point);
				marqueur3.transform.LookAt(hit.point);
				if (joueurMain.moveSelected == 2)
				{
					marqueur2.transform.position = hit.point;
				}
				else if (joueurMain.moveSelected == 4)
				{
					marqueur4.transform.position = hit.point;
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
