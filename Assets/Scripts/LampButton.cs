using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstPersonMobileTools.DynamicFirstPerson;

public class LampButton : MonoBehaviour, IInteractable
{
    Vector3 lastCamPos;
    Quaternion lastCamRotation;
    bool currentStatic = false;

    public Camera StaticCamera;
    public GameObject Player;
    
    
    public void interact()
    {
        if (currentStatic == false)
        {
            lastCamPos = Camera.main.transform.position;
            lastCamRotation = Camera.main.transform.rotation;

            Camera.main.transform.position = StaticCamera.transform.position;
            Camera.main.transform.rotation = StaticCamera.transform.rotation;

            Player.GetComponent<CharacterController>().enabled = false;
            Player.GetComponent<CameraLook>().enabled = false;

            currentStatic = true;
        }
        else
        {
            Camera.main.transform.position = lastCamPos;
            Camera.main.transform.rotation = lastCamRotation;

            Player.GetComponent<CharacterController>().enabled = true;
            Player.GetComponent<CameraLook>().enabled = true;

            currentStatic = false;
        }


        
    }
}
