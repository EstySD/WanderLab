using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour, IInteractable
{
    public GameObject door;
    DoorRotate doorRotate;
    // Start is called before the first frame update
    void Start()
    {
        doorRotate = door.GetComponent<DoorRotate>();
    }

    public void interact()
    {
            if(doorRotate.IsOpen){
                doorRotate.rotate=false;
            }
            else{
                doorRotate.rotate=true;
            }
    }
}
