using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zonePoison : MonoBehaviour
{
    ObjectTemporaire mainObject;
	public ParticleSystem particules;

	// Start is called before the first frame update
	void Start()
    {
        mainObject = GetComponent<ObjectTemporaire>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainObject.updateEffects == true)
        {
            GameManager.singleton.changeLevelPoison(-1);
			UpdateParticules();

			if (GameManager.singleton.levelPoison == 0)
            {
				GameManager.singleton.ResetPoison();
                transform.position = new Vector3(0f, -120f, 0f);
                gameObject.SetActive(false);
            }
            mainObject.updateEffects = false;
        }
    }


	public void UpdateParticules()
	{

		var emission = particules.emission;
		switch (GameManager.singleton.levelPoison)
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

	public int getPoisonStrength()
    {
        switch (GameManager.singleton.levelPoison)
        {
            case 0:
                return 0;
            case 1:
                return 5;
            case 2:
                return 10;
            case 3:
                return 15;

            default:
                return 0;
        }

    }
}
