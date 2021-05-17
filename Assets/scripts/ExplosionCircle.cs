using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionCircle : MonoBehaviour
{
	float timerDestruction = 0;
	public bool isDamage;
	public int healAmount;
	public int damageAmount;

	public GameObject particlesDamage;
	public GameObject particlesHeal;
	// Start is called before the first frame update
	void Start()
    {

		if(isDamage == false)
		{
			particlesDamage.SetActive(false);
			particlesHeal.SetActive(true);
		}
		/*
		else
		{
			Collider[] colliders = Physics.OverlapSphere(transform.position, 10);
			foreach (Collider item in colliders)
			{
				Rigidbody rb = item.GetComponent<Rigidbody>();

				if (rb != null)
				{
					//Appliquer une vélocité
					rb.AddExplosionForce(10, transform.position, 10, 1, ForceMode.Impulse);
				}
			}
		}*/
		
	}

    // Update is called once per frame
    void Update()
    {
		timerDestruction += Time.deltaTime;
		if (timerDestruction > 0.5)
		{
			Destroy(gameObject);
		}
	}
}
