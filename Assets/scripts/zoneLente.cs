using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoneLente : MonoBehaviour
{
 
    ObjectTemporaire mainObject;
    // Start is called before the first frame update
    void Start()
    {
        mainObject = GetComponent<ObjectTemporaire>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mainObject.updateEffects == true)
        {
            GameManager.singleton.changeLevelSlow(-1);

            if(GameManager.singleton.levelSlow == 0)
            {
                transform.position = new Vector3(0f, -120f, 0f);
                gameObject.SetActive(false);
            }
            mainObject.updateEffects = false;
        }
    }


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
