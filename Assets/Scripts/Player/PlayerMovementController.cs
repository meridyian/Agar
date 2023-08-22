using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : NetworkBehaviour
{
    public static PlayerMovementController Local { get; set; }
    private PlayerStateController _playerStateController;
    
    // camera settings
    public CinemachineFreeLook localCamera;
    private Transform cameraMainTransform;
    
    // other components
    private Rigidbody rb;
    public bool splitPressed;

    
    //movement
    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector3 m_movement;
    private float playerSpeed = 3f;

    // for collision detection

     void Awake()
    {
        rb = transform.GetChild(0).GetComponent<Rigidbody>();
        localCamera = transform.GetChild(1).GetComponent<CinemachineFreeLook>();
        controls = new PlayerControls();
       

    }

    public override void Spawned()
    {
        // input authority is checked to see who is your local player to assign this script
        // to make maincamera able to render the camera of local player make it free from its parent when it is the local 
        // disable otherwise
        if (Object.HasStateAuthority)
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
        
        if (Object.HasStateAuthority)
        {
            moveInput = controls.Player.Move.ReadValue<Vector2>();
            m_movement.Set(moveInput.x, 0f,moveInput.y);
            m_movement = Quaternion.AngleAxis(cameraMainTransform.eulerAngles.y, Vector3.up) * m_movement;
            //m_movement = cameraMainTransform.forward * m_movement.z + cameraMainTransform.right * m_movement.x;
            //m_movement.y = 0f;
            rb.AddForce(m_movement * playerSpeed);
            
        }
        

    }



}
