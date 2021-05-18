using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeDeFeu : MonoBehaviour
{
	float timerDestruction = 0f;

	/// <summary>
	/// Détruit le cone après un certain temps
	/// </summary>
	private void FixedUpdate()
	{

		timerDestruction += Time.deltaTime;
		if (timerDestruction > 2.5f)
		{
			Destroy(gameObject);
		}
	}
}
