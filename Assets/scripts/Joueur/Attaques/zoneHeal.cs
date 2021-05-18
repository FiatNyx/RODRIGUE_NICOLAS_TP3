using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoneHeal : MonoBehaviour
{
    ObjectTemporaire mainObject;
	public ParticleSystem particules;
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
        if (mainObject.updateEffects == true)
        {
            GameManager.singleton.changeLevelHeal(-1);
			UpdateParticules();

			if (GameManager.singleton.levelHeal == 0)
            {
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
		switch (GameManager.singleton.levelHeal)
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
	/// La puissance de heal du cercle
	/// </summary>
	/// <returns></returns>
    public int getHealStrength()
    {
        switch (GameManager.singleton.levelHeal)
        {
            case 0:
                return 0;
            case 1:
                return 10;
            case 2:
                return 15;
            case 3:
                return 20;

            default:
                return 0;
        }
    }
}
