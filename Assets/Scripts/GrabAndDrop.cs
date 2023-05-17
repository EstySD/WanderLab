using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndDrop : MonoBehaviour, IInteractable
{
   [SerializeField] private LayerMask PickupMask;
    [SerializeField] private Camera PlayerCam;
    [SerializeField] private Transform PickupTarget;
    [SerializeField] private CharacterController CharController;
    [Space]
    [SerializeField] private float PickupRange;
    private Rigidbody CurrentObject;
        
    
    public void interact()
    {
        if(CurrentObject)
        {
            CurrentObject.useGravity = true;
            Unselect();
            return;
        }

        Ray CameraRay = PlayerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
        if (Physics.Raycast(CameraRay, out RaycastHit HitInfo, PickupRange, PickupMask))
        {
            CurrentObject = HitInfo.rigidbody;
            CurrentObject.useGravity = false;
            CurrentObject.isKinematic = false;
            Physics.IgnoreCollision(CurrentObject.GetComponent<Collider>(), CharController.GetComponent<Collider>());
        }
    }

    void FixedUpdate()
    {
        
        if(CurrentObject)
        {
            Vector3 DirectionToPoint = PickupTarget.position - CurrentObject.position;
            float DistanceToPoint = DirectionToPoint.magnitude;

            CurrentObject.velocity = DirectionToPoint * 30f * DistanceToPoint; 
        }
        
    }
    public void Unselect(){
        CurrentObject = null;
    }
}
