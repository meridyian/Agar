using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : NetworkBehaviour
{
    
    // This class controls only movement related processes
    
    public static PlayerMovementController Local { get; set; }
    private PlayerStateController _playerStateController;
    
    // camera settings
    public CinemachineVirtualCamera localCamera;
    private Transform cameraMainTransform;
    
    // other components
    private Rigidbody rb;
    
    //movement
    private PlayerControls controls;
    private Vector2 moveInput;
    public Vector3 m_movement;
    private float playerSpeed = 3f;

    // for collision detection

     void Awake()
    {
        rb = transform.GetChild(0).GetComponent<Rigidbody>();
        localCamera = transform.GetChild(1).GetComponent<CinemachineVirtualCamera>();
        controls = new PlayerControls();
       

    }

    public override void Spawned()
    {
        // state authority is checked to see who is your local player to assign this script
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
            rb.AddForce(m_movement * playerSpeed);
            
        }
        

    }



}
