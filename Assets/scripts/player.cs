using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class player : MonoBehaviour
{
	Camera mainCam;
	
	int moveSelected = 0;
	public ParticleSystem particles;
	Animator animationJoueur;
	bool isRotating;

	int vie = 30;
	public int vieMax = 30;
	Vector3 rotationTarget;


	//values for internal use
	private Quaternion _lookRotation;
	private Vector3 _direction;
	Vector3 cible;

	public GameObject fireball;
	public GameObject marqueur1;
	public GameObject marqueur2;
	public GameObject marqueur3;
	public GameObject marqueur4;

	public Transform projectileStartPoint;
	public bool isAttacking = false;
	Vector3 moveDirection;
	Rigidbody rb;
	public GameObject cercleLentPrefab;

	public Transform cameraPosition;

	bool isSlowed = false;
	bool isPoisoned = false;
	float timerPoison = 0;
	public GameObject eclair;
	AudioSource audioSource;
	public AudioClip audioDamage;
	public AudioClip audioEclair;
	public AudioClip audioZoom;
	public AudioClip audioBoom;
	
	public ParticleSystem teleportParticles;
	
	/// <summary>
	/// Initialisation de la variable de caméra et changement du texte de vie dans l'UI pour la vie du personnage
	/// </summary>
	void Start()
	{
		mainCam = Camera.main;
		UI_Manager.singleton.changeVieText(vieMax, vie);
	}

	/// <summary>
	/// Stock certains component dans des variables et initialise la vie selon la vie maximale
	/// </summary>
	private void Awake()
	{
		
		animationJoueur = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		vie = vieMax;
		audioSource = GetComponent<AudioSource>();
	}

	/// <summary>
	/// S'occupe des attaques et de chaque action que le joueur peut faire avec ses inputs
	/// </summary>
	void Update()
    {

		//S'assure que la caméra suit le personnage
		cameraPosition.position = transform.position;


		//Pour s'assurer que le personnage cours quand il se déplace
		animationJoueur.SetBool("Idle", false);

		//S'il s'agit du tour du personnage
		if (GameManager.singleton.getPlayerTurn() && isAttacking == false)
		{
			
			//Tourne la caméra. Ne peut pas rien tourner le personnage en même temps de tourner la caméra.
            if (Input.GetMouseButton(2))
            {
				RotateCamera();
            }
            else
            {
				//Tourner le personnage pour qu'il face face à la souris
				Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);

				RaycastHit hit;

				if (Physics.Raycast(camRay, out hit))
				{
				
					Vector3 lookDirection = hit.point;
					lookDirection.y = transform.position.y;

					Vector3 relativePos = lookDirection - transform.position;

				
					Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
					transform.rotation = rotation;
				}
			}
			
			//Clic droit pour annuler une attaque
            if (Input.GetMouseButtonDown(1))
            {
				resetAttackSelected();
			}



			//--------------------
			//Sélection des attaques
			//-------------------
			if (Input.GetKeyDown(KeyCode.Alpha1))
			{
				effacerMarqueurs();
				marqueur1.SetActive(true);
				marqueur1.transform.position = transform.position;
				moveSelected = 1;
				UI_Manager.singleton.changeSelectedMove(1);
			
			}
			else if (Input.GetKeyDown(KeyCode.Alpha2))
			{
				effacerMarqueurs();
				Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);

				RaycastHit hit;

				if (Physics.Raycast(camRay, out hit))
				{
					marqueur2.SetActive(true);
					marqueur2.transform.position = hit.point;
					
				}
				moveSelected = 2;
				UI_Manager.singleton.changeSelectedMove(2);
			}
			else if (Input.GetKeyDown(KeyCode.Alpha3))
			{
				effacerMarqueurs();
				marqueur3.SetActive(true);
				marqueur3.transform.position = transform.position;
				moveSelected = 3;
				UI_Manager.singleton.changeSelectedMove(3);
				
			}
			else if (Input.GetKeyDown(KeyCode.Alpha4))
			{
				effacerMarqueurs();
				Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);

				RaycastHit hit;

				if (Physics.Raycast(camRay, out hit))
				{
					marqueur4.SetActive(true);
					marqueur4.transform.position = hit.point;
					
				}

				moveSelected = 4;
				UI_Manager.singleton.changeSelectedMove(4);
				
			}



			//Déplacement du personnage
			if(moveSelected == 0)
            {
				float inputVertical = Input.GetAxis("Vertical");

				float inputHorizontal = Input.GetAxis("Horizontal");

				

				if (isPoisoned && (Mathf.Abs(inputHorizontal) > 0 || Mathf.Abs(inputVertical) > 0))
				{
					timerPoison += Time.deltaTime;

					if(timerPoison > 0.5)
					{
						damage(3);
						timerPoison = 0;
					}
				}

			
				moveDirection = cameraPosition.forward * inputVertical + cameraPosition.right * inputHorizontal;
				

				


			}//Si une attaque a été sélectionnée
            else
            {
				//S'assure que le personnage ne se déplace pas
				moveDirection = Vector3.zero;
				animationJoueur.SetBool("Idle", true);


				Ray camRay = mainCam.ScreenPointToRay(Input.mousePosition);

				RaycastHit hit;

				if (Physics.Raycast(camRay, out hit))
				{
					//S'occupe de la rotation de tous les marqueurs. Ainsi que leur position
					projectileStartPoint.LookAt(hit.point);
					marqueur1.transform.LookAt(hit.point);
					marqueur3.transform.LookAt(hit.point);
					if (moveSelected == 2)
					{
						marqueur2.transform.position = hit.point;
					}
					else if(moveSelected == 4)
					{
						marqueur4.transform.position = hit.point;
					}
				}

				

				//--------------------------------
				//Section des attaques. Le getTimerJoueur() est pour s'assurer que le joueur ait assez de temps pour payer l'attaque
				//---------------------------------
				if (Input.GetMouseButtonDown(0))
				{
					if (moveSelected == 1 && GameManager.singleton.getTimerJoueur() > 2)
					{
						isAttacking = true;
						animationJoueur.SetTrigger("FireballAttack");
						StartCoroutine(BouleDeFeu());
						
					}
					else if(moveSelected == 2 && GameManager.singleton.getTimerJoueur() > 4)
					{
						

						if (Physics.Raycast(camRay, out hit))
						{
							GameManager.singleton.StartAttack(4);
							GameObject cercleLent = Instantiate(cercleLentPrefab, hit.point, transform.rotation);
							
							GameManager.singleton.FinishAttack();
						}
					}
					else if(moveSelected == 3 && GameManager.singleton.getTimerJoueur() > 3)
					{
						isAttacking = true;
						animationJoueur.SetTrigger("LightningAttack");
						StartCoroutine(Eclair());
						audioSource.PlayOneShot(audioEclair);
					}
					else if(moveSelected == 4 && GameManager.singleton.getTimerJoueur() > 6)
					{
					
						if (Physics.Raycast(camRay, out hit))
						{
							GameManager.singleton.StartAttack(6);
							transform.position = hit.point;
							teleportParticles.Play();
							teleportParticles.GetComponent<TeleportParticles>().Spin();
							GameManager.singleton.FinishAttack();
							audioSource.PlayOneShot(audioZoom);
						}

					
					}
				}
			}
        }
		//S'il s'agit du tour de l'ennemi, s'assure que le personnage ne peut plus bouger ou attaquer, mais permet de tourner la caméra
        else
        {
			resetAttackSelected();
			moveDirection = Vector3.zero;
			animationJoueur.SetBool("Idle", true);

			if (Input.GetMouseButton(2))
			{
				RotateCamera();
			}
		}
	}

	/// <summary>
	/// Efface tous les marqueurs et remet l'attaque sélectionnée à rien
	/// </summary>
	void resetAttackSelected()
	{
		moveSelected = 0;
		effacerMarqueurs();
		UI_Manager.singleton.changeSelectedMove(0);
	}

	/// <summary>
	/// Endommage le personnage et reload la scène s'il est tué
	/// </summary>
	/// <param name="damage"></param>
	public void damage(int damage)
	{
		vie -= damage;
		
		UI_Manager.singleton.changeVieText(vieMax, vie);
		audioSource.PlayOneShot(audioDamage);

		if (vie <= 0)
		{
			Scene scene = SceneManager.GetActiveScene();
			SceneManager.LoadScene(scene.name);
		}
	}

	/// <summary>
	/// Efface les marqueurs
	/// </summary>
	private void effacerMarqueurs()
	{
		marqueur1.SetActive(false);
		marqueur2.SetActive(false);
		marqueur3.SetActive(false);
		marqueur4.SetActive(false);
	}

	/// <summary>
	/// Déplace le personnage
	/// </summary>
    private void FixedUpdate()
    {
		float speed = 5f;
		if (isSlowed)
		{
			speed /= 2f;
		}
		Vector3 oldPosition = rb.position;
		rb.MovePosition(rb.position + moveDirection.normalized * speed * Time.fixedDeltaTime);

		Vector3 deplacement = rb.position - oldPosition;
		
		float angle = Quaternion.Angle(cameraPosition.rotation, transform.rotation);
		deplacement = Quaternion.AngleAxis(-angle, Vector3.up) * deplacement;
		deplacement = deplacement.normalized;
		animationJoueur.SetFloat("horizontal", deplacement.x);
		animationJoueur.SetFloat("vertical", deplacement.z);
		print(deplacement);
	}

	/// <summary>
	/// Détecte si le personnage est dans une zone de poison/ralentissement
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "LentPoison")
		{
			isSlowed = true;
			isPoisoned = true;
		}
	}

	/// <summary>
	/// Détecte si le personnage est sortit d'une zone de poison/ralentissement
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "LentPoison")
		{
			isSlowed = false;
			isPoisoned = false;
		}
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

	/// <summary>
	/// S'occupe du lancement de l'attaque de boule de feu
	/// </summary>
	/// <returns></returns>
	IEnumerator BouleDeFeu()
	{
		GameManager.singleton.StartAttack(2);

		float timerMove = 0;
		while (isAttacking && timerMove < 1.5f)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}

		
		
		GameObject bouleDeFeu = Instantiate(fireball, projectileStartPoint.position, projectileStartPoint.rotation);
		bouleDeFeu.GetComponent<Fireball>().joueur = this;

		timerMove = 0;
		while (isAttacking && timerMove < 2)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}

		isAttacking = false;
		GameManager.singleton.FinishAttack();
	}

	/// <summary>
	/// S'occupe du lancement de l'attaque d'éclair
	/// </summary>
	/// <returns></returns>
	IEnumerator Eclair()
	{

		float timerMove = 0;
		GameManager.singleton.StartAttack(3);

		while (isAttacking && timerMove < 1)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}
		GameObject eclairInstance = Instantiate(eclair, projectileStartPoint.position, projectileStartPoint.rotation);
		eclairInstance.GetComponent<Eclair>().joueur = this;

		timerMove = 0;
		while (isAttacking && timerMove < 2)
		{
			timerMove += Time.deltaTime;
			yield return null;
		}

		isAttacking = false;
		GameManager.singleton.FinishAttack();
	}
}
