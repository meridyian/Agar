using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class PlayerMovementController : NetworkBehaviour
{
    public static PlayerMovementController Local { get; set; }
    
    // camera settings
    public CinemachineFreeLook localCamera;
    private Transform cameraMainTransform;
    
    // other components
    private Rigidbody rb;
    
    //movement
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 m_movement;

    // for collision detection
    [SerializeField] private Collider[] playerHitColliders;

    private void Awake()
    {
        rb = transform.GetChild(0).GetComponent<Rigidbody>();
        localCamera = GetComponentInChildren<CinemachineFreeLook>();
        controls = new PlayerControls();

    }

    public override void Spawned()
    {
        // input authority is checked to see who is your local player to assign this script
        // to make maincamera able to render the camera of local player make it free from its parent when it is the local 
        // disable otherwise
        if (Object.HasInputAuthority)
        {
            Local = this;
            localCamera.transform.parent = null;
            controls.Player.Enable();
            cameraMainTransform = Camera.main.transform;
        }
        else
        {
            localCamera.enabled = false;
        }
    }

    public override void FixedUpdateNetwork()
    {
        // burda neden state check ettik ?? 
        
        if (Object.HasStateAuthority)
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
            
            m_movement.Set(moveInput.x, 0f,moveInput.y);
            m_movement = cameraMainTransform.forward * m_movement.z + cameraMainTransform.right * m_movement.x;
            m_movement.y = 0f;
            rb.AddForce(m_movement);








        }
    }
}
