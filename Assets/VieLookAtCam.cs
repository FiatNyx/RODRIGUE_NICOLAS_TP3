using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VieLookAtCam : MonoBehaviour
{

    Transform cameraMain;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        cameraMain = Camera.main.GetComponent<Transform>();
        target = GameManager.singleton.cameraViePosition;
    }

    // Update is called once per frame
    void Update()
    {
        target.position = new Vector3(cameraMain.position.x, transform.position.y, cameraMain.position.z);
        
        transform.LookAt(target);
    }
}
