using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerStateController : NetworkBehaviour
{
    public bool isBot;

    [Networked(OnChanged = nameof(NetworkSizeChanged))]
    public float playerSize { get; set; }

    private Rigidbody rb;
    private bool splitPressed;

    private float splitThreshold = 2f;

    public GameObject splittedPiecePref;
    [SerializeField] private LayerMask foodLayerMask;
    [SerializeField] List<GameObject> foodTarget;
    [SerializeField] Collider[] hitcolliders;
    [SerializeField] List<NetworkObject> splittedPieces = new List<NetworkObject>();


    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //_playerControls = gameObject.GetComponentInParent<InputAction>();


    }

    // size senin kendi objende değişiyo ama networkte göstermelisin networked
    // property olmalı



    public override void FixedUpdateNetwork()
    {
        MovePartsTogether();
    }


    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("collided with food");
            playerSize += playerSize * 0.1f;
            rb.transform.localScale = new Vector3(playerSize, playerSize, playerSize);
            other.gameObject.transform.position = Utils.GetRandomSpawnPosition(other.transform.localScale.x);
        }

        /*
        // check if colliding with obstacle
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("collided with obstacle");
            // call split function
        }
        */
    }

    private static void NetworkSizeChanged(Changed<PlayerStateController> changed)
    {
        changed.Behaviour.playerSize = changed.Behaviour.playerSize;

    }


    public void OnSplit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
            if (playerSize > splitThreshold)
            {
                // YOU CAN SPAWN PLAYERBODY IN HERE

                SpaceSplit(); 
           
            }

            Debug.Log("you are not big enough");

        }

    }

    public void SpaceSplit()
    {
        splittedPieces.Add(transform.GetComponent<NetworkObject>());
        playerSize -= playerSize * 0.3f;
        transform.localScale = new Vector3(playerSize, playerSize, playerSize);
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, transform.position,
            Quaternion.identity);
        splitPiece.GetComponent<Rigidbody>().AddForce(Vector3.forward, ForceMode.Impulse);
        splitPiece.transform.parent = transform;
        splittedPieces.Add(splitPiece);
    }

    public void MovePartsTogether()
    {
        if (splittedPieces.Count >=1)
        {
            Vector3 centerpos = Vector3.zero;
            foreach (NetworkObject splittedBall in splittedPieces)
            {
                centerpos += splittedBall.transform.position;
            }

            centerpos /= splittedPieces.Count;

            foreach (NetworkObject ball in splittedPieces)
            {
                Rigidbody rb = ball.GetComponent<Rigidbody>();
                Vector3 moveDir = (centerpos - rb.transform.position).normalized;
                rb.AddForce(moveDir * 2f  ,ForceMode.Impulse);
            }
        }
    }


  
}