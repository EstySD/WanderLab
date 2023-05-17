using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstPersonMobileTools;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private LayerMask PickupMask;
    [SerializeField] private Camera PlayerCam;
    [SerializeField] private float PickupRange;

    public MobileButton button;
    private Rigidbody CurrentObject;

    

    // Update is called once per frame
    void Start()
    {
        button.m_OnClicked.AddListener(ko);
    }

    public void ko()
    {
        
        Ray CameraRay = PlayerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
        if (Physics.Raycast(CameraRay, out RaycastHit HitInfo, PickupRange, PickupMask))
        {
            Debug.Log(HitInfo);
            Debug.Log(HitInfo.collider.name);
            IInteractable interactor = (IInteractable)HitInfo.collider.GetComponent(typeof(IInteractable));
            interactor.interact();
        }
    }
}
