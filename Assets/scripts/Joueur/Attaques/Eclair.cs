using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eclair : MonoBehaviour
{

	float timerDestruction = 0f;
	public JoueurMain joueur;

	/// <summary>
	/// Détruit l'éclair après un certain temps et avertit le joueur que l'attaque est finie. Grossis l'éclair au fil du temps
	/// </summary>
	private void FixedUpdate()
	{
		transform.localScale += Vector3.forward * 10 * Time.fixedDeltaTime;
		
		timerDestruction += Time.deltaTime;
		if (timerDestruction > 0.5f)
		{
			joueur.isAttacking = false;
			Destroy(gameObject);
		}
	}

	
}
