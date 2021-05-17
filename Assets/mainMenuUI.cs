using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mainMenuUI : MonoBehaviour
{
	public GameObject menuPrincipal;
	public GameObject menuOptions;
	public GameObject menuChoixNiveau;
	public GameObject menuCommentJouer;

	public GameObject[] listePagesHowToPlay;

	int pageIndex = 0;

	public Dropdown dropDownQualite;
    // Start is called before the first frame update
    void Start()
    {
		dropDownQualite.value = QualitySettings.GetQualityLevel() - 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	public void onOptionPressed()
	{
		menuPrincipal.SetActive(false);
		menuOptions.SetActive(true);
	}

	public void onPlayPressed()
	{
		menuPrincipal.SetActive(false);
		menuChoixNiveau.SetActive(true);

	}

	public void onHowToPlayPressed()
	{

		menuPrincipal.SetActive(false);
		menuCommentJouer.SetActive(true);
		resetHelpPage();
		pageIndex = 0;
		listePagesHowToPlay[0].SetActive(true);
	}


	public void onPagePrecedantePressed()
	{
		resetHelpPage();

		pageIndex -= 1;

		if(pageIndex < 0)
		{
			pageIndex = 0;
		}

		listePagesHowToPlay[pageIndex].SetActive(true);
	}

	public void onPageSuivantePressed()
	{
		resetHelpPage();

		pageIndex += 1;

		if (pageIndex > listePagesHowToPlay.Length - 1)
		{
			pageIndex = listePagesHowToPlay.Length - 1;
		}

		listePagesHowToPlay[pageIndex].SetActive(true);
	}


	public void onRetourFromCommentJouerPressed()
	{
		menuPrincipal.SetActive(true);
		menuCommentJouer.SetActive(false);
	}

	public void onRetourFromOptionsPressed()
	{
		menuPrincipal.SetActive(true);
		menuOptions.SetActive(false);

		
		QualitySettings.SetQualityLevel(dropDownQualite.value + 1);
			
		
	}

	public void onRetourFromJouerPressed()
	{
		menuPrincipal.SetActive(true);
		menuChoixNiveau.SetActive(false);
	}

	private void resetHelpPage()
	{
		foreach (GameObject page in listePagesHowToPlay)
		{
			page.SetActive(false);
		}

	}
}
