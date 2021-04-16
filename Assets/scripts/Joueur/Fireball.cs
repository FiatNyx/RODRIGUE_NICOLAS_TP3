using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
	Rigidbody rb;
	public JoueurMain joueur;

	public GameObject explosion;
	float timerDestruction = 0;

	/// <summary>
	/// Initialisation de certaines variables
	/// </summary>
    void Start()
    {
		rb = GetComponent<Rigidbody>();
		timerDestruction = 0;
		
    }

	/// <summary>
	/// Déplace la boule de feu et la détruit après un certain temps et avertit le joueur que l'attaque est finie.
	/// Instancie une explosion quand la boule de feu est détruite.
	/// </summary>
	private void FixedUpdate()
	{
		rb.MovePosition(transform.position + transform.forward * Time.deltaTime * 10);
		timerDestruction += Time.deltaTime;
		if(timerDestruction > 2)
		{
			GameObject explosionInstance = Instantiate(explosion, transform.position, transform.rotation);
			explosionInstance.GetComponent<Explosion>().joueur = joueur;
			
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Détecte si la boule de feu est entrée en collision. 
	/// Si l'autre objet est un ennemi, lui applique des dégats.
	/// Instancie une explosion et détruit la boule de feu.
	/// </summary>
	/// <param name="collision">La collision, ce qui permet d'accéder à l'autre objet de la collision</param>
	private void OnCollisionEnter(Collision collision)
	{
		if(collision.collider.GetComponent<ennemyBasic>() != null)
		{
			collision.collider.GetComponent<ennemyBasic>().dealDamage(10);
			
		}

		GameObject explosionInstance = Instantiate(explosion, transform.position, transform.rotation);
		
		explosionInstance.GetComponent<Explosion>().joueur = joueur;
		Destroy(gameObject);
	}
}
