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

    /// <summary>
	/// Détruit le mur de feu au prochain tour
	/// </summary>
    void Update()
    {
        if (mainObject.updateEffects == true)
        {
            Destroy(gameObject);
        }
    }
}
