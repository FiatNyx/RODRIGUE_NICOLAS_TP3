using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoneLente : MonoBehaviour
{
	public ParticleSystem particules;
	ObjectTemporaire mainObject;
    // Start is called before the first frame update
    void Start()
    {
        mainObject = GetComponent<ObjectTemporaire>();
    }


	/// <summary>
	/// Enlève 1 niveau à chaque début de tour et suprime le cercle s'il atteint le niveau  
	/// </summary>
	void Update()
    {
        if(mainObject.updateEffects == true)
        {
            GameManager.singleton.changeLevelSlow(-1);
			UpdateParticules();

			if (GameManager.singleton.levelSlow == 0)
            {
				GameManager.singleton.ResetSlow();
                transform.position = new Vector3(0f, -120f, 0f);
                gameObject.SetActive(false);
            }
            mainObject.updateEffects = false;
        }
    }

	/// <summary>
	/// Change le nombre de particules selon le niveau de la zone
	/// </summary>
	public void UpdateParticules()
	{

		var emission = particules.emission;
		switch (GameManager.singleton.levelSlow)
		{

			case 1:
				emission.rateOverTime = 1;
				break;
			case 2:

				emission.rateOverTime = 5;
				break;

			case 3:
				emission.rateOverTime = 10;
				break;
		}
	}

	/// <summary>
	/// La puissance de ralentissement du cercle
	/// </summary>
	/// <returns></returns>
	public float getSlowStrength()
    {
        switch (GameManager.singleton.levelSlow)
        {
            case 0:
                return 0;
            case 1:
                return 1.5f;
            case 2:
                return 2f;
            case 3:
                return 2.5f;

            default:
                return 0;
        }
    }
}
