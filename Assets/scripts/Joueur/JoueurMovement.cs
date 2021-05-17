using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurMovement : MonoBehaviour
{



    JoueurMain joueurMain;
	Vector3 moveDirection;

	float animationSpeed;
	Rigidbody rb;
	public bool isMoving = false;
	// Start is called before the first frame update
	void Start()
    {
        joueurMain = GetComponent<JoueurMain>();
		animationSpeed = joueurMain.animationJoueur.speed;
    }

    private void Awake()
    {
		rb = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
	{
		if(joueurMain.isDead == false && GameManager.singleton.isPaused == false)
        {
			if (joueurMain.isThisPlayersTurn && joueurMain.isAttacking == false) {
				//S'assure que la caméra suit le personnage
				GameManager.singleton.cameraPosition.position = transform.position;

				RaycastHit hit;
				if (Physics.Raycast(joueurMain.camRay, out hit))
				{

					Vector3 lookDirection = hit.point;
					lookDirection.y = transform.position.y;

					Vector3 relativePos = lookDirection - transform.position;


					Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
					transform.rotation = rotation;
				}

				//Tourne la caméra. Ne peut pas rien tourner le personnage en même temps de tourner la caméra.
				if (Input.GetMouseButton(2))
				{
					RotateCamera();
				}

				//Déplacement du personnage
				if (joueurMain.moveSelected == 0)
				{
					float inputVertical = Input.GetAxis("Vertical");

					float inputHorizontal = Input.GetAxis("Horizontal");

					if (Mathf.Abs(inputVertical) > 0 || Mathf.Abs(inputHorizontal) > 0) 
					{
						isMoving = true;
					}
					else
					{
						isMoving = false;

					}

					if (joueurMain.isPoisoned && (Mathf.Abs(inputHorizontal) > 0 || Mathf.Abs(inputVertical) > 0))
					{
						joueurMain.timerPoison += Time.deltaTime;

						print(joueurMain.timerPoison);
						if (joueurMain.timerPoison > 0.5)
						{
							joueurMain.damage(joueurMain.puissancePoison);
							joueurMain.timerPoison = 0;
						}
					}


					moveDirection = GameManager.singleton.cameraPosition.forward * inputVertical + GameManager.singleton.cameraPosition.right * inputHorizontal;


				}
				else
				{
					moveDirection = Vector3.zero;
					isMoving = false;
					//joueurMain.animationJoueur.SetBool("Idle", true);
				}
			}
			else
			{
				moveDirection = Vector3.zero;
				//joueurMain.animationJoueur.SetBool("Idle", true);

				if (Input.GetMouseButton(2))
				{
					RotateCamera();
				}
			}

		}

	}

	/// <summary>
	/// Déplace le personnage
	/// </summary>
	private void FixedUpdate()
	{
		if(joueurMain.isDead == false)
        {
			float speed = 5f;
			joueurMain.animationJoueur.speed = animationSpeed;
			if (joueurMain.isSlowed)
			{
				speed /= 2f;
				joueurMain.animationJoueur.speed = animationSpeed / 2f;
			}

			rb.MovePosition(rb.position + moveDirection.normalized * speed * Time.fixedDeltaTime);


			moveDirection = moveDirection.normalized;
			Vector3 animDir = transform.InverseTransformDirection(moveDirection);

			joueurMain.animationJoueur.SetFloat("horizontal", animDir.x);
			joueurMain.animationJoueur.SetFloat("vertical", animDir.z);
		}
		

	}

	/// <summary>
	/// Permet de tourner la caméra
	/// </summary>
	void RotateCamera()
	{
		Vector3 rotCamera = GameManager.singleton.cameraPosition.rotation.eulerAngles;


		rotCamera.y += Input.GetAxis("Mouse X") * 5;


        GameManager.singleton.cameraPosition.rotation = Quaternion.Euler(rotCamera);
	}

}
