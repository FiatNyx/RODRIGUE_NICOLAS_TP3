using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurMovement : MonoBehaviour
{


    public Transform cameraPosition;

    JoueurMain joueurMain;
	Vector3 moveDirection;

	
	Rigidbody rb;

	// Start is called before the first frame update
	void Start()
    {
        joueurMain = GetComponent<JoueurMain>();
    }

    private void Awake()
    {
		rb = GetComponent<Rigidbody>();
	}

    // Update is called once per frame
    void Update()
	{

		//S'assure que la caméra suit le personnage
		cameraPosition.position = transform.position;

		if (GameManager.singleton.getPlayerTurn() && joueurMain.isAttacking == false) {

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



				if (joueurMain.isPoisoned && (Mathf.Abs(inputHorizontal) > 0 || Mathf.Abs(inputVertical) > 0))
				{
					joueurMain.timerPoison += Time.deltaTime;

					if (joueurMain.timerPoison > 0.5)
					{
						joueurMain.damage(3);
						joueurMain.timerPoison = 0;
					}
				}


				moveDirection = cameraPosition.forward * inputVertical + cameraPosition.right * inputHorizontal;


            }
            else
            {
				moveDirection = Vector3.zero;
				joueurMain.animationJoueur.SetBool("Idle", true);
			}
		}
        else
        {
			moveDirection = Vector3.zero;
			joueurMain.animationJoueur.SetBool("Idle", true);

			if (Input.GetMouseButton(2))
			{
				RotateCamera();
			}
		}
	}

	/// <summary>
	/// Déplace le personnage
	/// </summary>
	private void FixedUpdate()
	{
		float speed = 5f;
		if (joueurMain.isSlowed)
		{
			speed /= 2f;
		}

		rb.MovePosition(rb.position + moveDirection.normalized * speed * Time.fixedDeltaTime);


		moveDirection = moveDirection.normalized;
		Vector3 animDir = transform.InverseTransformDirection(moveDirection);

		joueurMain.animationJoueur.SetFloat("horizontal", animDir.x);
		joueurMain.animationJoueur.SetFloat("vertical", animDir.z);

	}

	/// <summary>
	/// Permet de tourner la caméra
	/// </summary>
	void RotateCamera()
	{
		Vector3 rotCamera = cameraPosition.rotation.eulerAngles;


		rotCamera.y += Input.GetAxis("Mouse X") * 10;


		cameraPosition.rotation = Quaternion.Euler(rotCamera);
	}

}
