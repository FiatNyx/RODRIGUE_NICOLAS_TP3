using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// La classe pour faire tourner les particules de la téléportation
/// </summary>
public class TeleportParticles : MonoBehaviour
{
	bool isSpinning;
	float timerSpin = 0f;


	/// <summary>
	///  Fait tourner les particules si le isSpinning est activé
	/// </summary>
	void Update()
    {
		if (isSpinning)
		{
			transform.Rotate(Vector3.forward * 3);
			timerSpin += Time.deltaTime;
			if(timerSpin >= 0.75f)
			{
				isSpinning = false;
			}
		}   
    }


	/// <summary>
	/// Active la rotation des particules
	/// </summary>
	public void Spin()
	{
		isSpinning = true;
		timerSpin = 0f;
	}
}
