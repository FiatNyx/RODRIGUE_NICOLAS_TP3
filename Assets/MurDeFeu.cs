using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MurDeFeu : MonoBehaviour
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
        if (mainObject.updateEffects == true)
        {
            Destroy(gameObject);
        }
    }
}
