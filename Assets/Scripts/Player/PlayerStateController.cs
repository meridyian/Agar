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

    private float splitThreshold = 1.3f;

    public GameObject splittedPiecePref;
    [SerializeField] private LayerMask foodLayerMask;
    [SerializeField] List<GameObject> foodTarget;
    [SerializeField] Collider[] hitcolliders;
    [SerializeField] List<NetworkObject> splittedPieces = new List<NetworkObject>();
    [SerializeField] private Transform InterpolationObj;
    private HingeJoint hingeJoint;


    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InterpolationObj = GetComponentInChildren<Transform>();
        //_playerControls = gameObject.GetComponentInParent<InputAction>();
        
    }

    // size senin kendi objende değişiyo ama networkte göstermelisin networked
    // property olmalı

    public override void Spawned()
    {


    }



    public override void FixedUpdateNetwork()
    {
        /*
foreach (NetworkObject splitPart in splittedPieces)
{
    if (splitPart != null)
    {
        Vector3 forceDirection = (InterpolationObj.position - splitPart.transform.position).normalized;
        splitPart.GetComponent<Rigidbody>().AddForce(forceDirection * 3f , ForceMode.Acceleration);
        
    }
}

if (splittedPieces.Counts > 0)
{
    var rigidb = splittedPieces[0].GetComponent<Rigidbody>();
    Vector3 moveDir = (rb.transform.position - rigidb.transform.position).normalized;
    rigidb.AddForce(moveDir * 0.1f, ForceMode.Impulse );
}
*/

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
        //splittedPieces.Add(transform.GetComponent<NetworkObject>());
        //playerSize -= playerSize * 0.3f;
        //Vector3 playersizeVector = new Vector3(playerSize, playerSize, playerSize);
        //transform.localScale = playersizeVector;
        /*
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, transform.position,
            Quaternion.identity);
        splitPiece.transform.localScale = playersizeVector;
        splitPiece.GetComponent<Rigidbody>().AddForce(Vector3.forward, ForceMode.Impulse);
        splitPiece.transform.parent = transform;
        splittedPieces.Add(splitPiece);
        */
        
        // calculate spawn position (forward from parent)
        Vector3 spawnPosition = transform.position + transform.position * 1.5f;
        // update player size and spawned piece size as needed
        playerSize -= playerSize * 0.3f;
        Vector3 playersizeVector = new Vector3(playerSize, playerSize, playerSize);

        // spawn the split object
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, transform.position,
            Quaternion.identity);
        splitPiece.transform.localScale = playersizeVector;
        // set the split object as a child of the parent 
        splitPiece.transform.parent = transform;
        
        
        // Update the hinge joint's anchors

        hingeJoint = splitPiece.GetComponent<Rigidbody>().AddComponent<HingeJoint>();
        hingeJoint.anchor = rb.position + Vector3.up * 2.0f;
        hingeJoint.connectedBody = rb;
        
        splittedPieces.Add(splitPiece);


    }


    
    /*

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
    */
    


  
}