using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class PlayerStateController : NetworkBehaviour
{
    public bool isBot;
    [Networked(OnChanged = nameof(NetworkSizeChanged))]
    public float playerSize{get; set;}

    private Rigidbody rb;
    
    public void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }

    // size senin kendi objende değişiyo ama networkte göstermelisin networked
    // property olmalı
    

    public void Update()
    {
        if (Input.GetButton("Jump"))
        {
            playerSize++;
            rb.transform.localScale = new Vector3(playerSize, playerSize, playerSize);
        }
    }
    
    private static void NetworkSizeChanged(Changed<PlayerStateController> changed)
    {
        changed.Behaviour.playerSize = changed.Behaviour.playerSize;
    }
}
