using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorRotate : MonoBehaviour
{
    public GameObject door;
    public bool IsOpen = false;
    public bool? rotate = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rotate== null){
        }
        else if(rotate==false){
            door.transform.Rotate(new Vector3(0,0,90));
            rotate= null;
            IsOpen = false;
        }
        else{
            door.transform.Rotate(new Vector3(0,0,-90));
            rotate= null;
            IsOpen = true;
        }
    }
}
