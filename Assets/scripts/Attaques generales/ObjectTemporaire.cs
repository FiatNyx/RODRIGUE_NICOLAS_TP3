using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTemporaire : MonoBehaviour
{

	//Une classe parente aux objets qui restent sur le terrain pendant un ou plusieurs tours. Permet au gameManager d'updater leurs effets
    public bool updateEffects = false;

    public void UpdateObjet()
    {
        updateEffects = true;
    }
}
