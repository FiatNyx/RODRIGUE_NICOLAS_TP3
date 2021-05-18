using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VieLookAtCam : MonoBehaviour
{

    Transform cameraMain;
    Transform target;
   

    void Start()
    {
        cameraMain = Camera.main.GetComponent<Transform>();
        target = GameManager.singleton.cameraViePosition;
    }

    // S'assure que les barres de vie des ennemis regardent la caméra.
    void Update()
    {
        target.position = new Vector3(cameraMain.position.x, transform.position.y, cameraMain.position.z);
        
        transform.LookAt(target);
    }
}
