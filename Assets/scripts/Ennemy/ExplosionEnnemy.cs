using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEnnemy : MonoBehaviour
{
	float timerDestruction = 0;

	/// <summary>
	/// Applique une explosion initiale qui affecte les rigidbodys
	/// </summary>
	void Start()
	{
		//Si on veut ajouter une force qui déplace les objets
		/*
		Collider[] colliders = Physics.OverlapSphere(transform.position, 10);
		foreach (Collider item in colliders)
		{
			Rigidbody rb = item.GetComponent<Rigidbody>();

			if (rb != null && item.tag != "Player")
			{
				//Appliquer une vélocité
				rb.AddExplosionForce(5, transform.position, 5, 1, ForceMode.Impulse);
			}
		}
		*/
	}

	/// <summary>
	/// Détruit l'explosion après un certain temps et avertit le joueur que l'attaque est finie.
	/// </summary>
	void Update()
	{
		timerDestruction += Time.deltaTime;
		if (timerDestruction > 0.5)
		{

			Destroy(gameObject);
		}
	}
}
