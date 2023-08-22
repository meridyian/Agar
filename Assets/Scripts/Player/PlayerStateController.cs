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
    public float splitRadius = 2f;
    private float splitThreshold = 2f;

    public GameObject splittedPiecePref;
    [SerializeField] private LayerMask foodLayerMask;
    [SerializeField] List<GameObject> foodTarget;
    [SerializeField] Collider[] hitcolliders;
    [SerializeField] List<NetworkObject> splittedPieces = new List<NetworkObject>();
    [SerializeField] List<NetworkObject> obstacleSplitted = new List<NetworkObject>();
    [SerializeField] private Transform InterpolationObj;


    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InterpolationObj = GetComponentInChildren<Transform>();

        //_playerControls = gameObject.GetComponentInParent<InputAction>();

    }

    // size senin kendi objende değişiyo ama networkte göstermelisin networked
    // property olmalı
    

    public override void FixedUpdateNetwork()
    {

        //MoveSplitCellsTogether();
        
        if (splittedPieces.Count > 0)
        {
            foreach (NetworkObject piece in splittedPieces)
            {
                Vector3 targetDir = InterpolationObj.position  - piece.transform.position;
                piece.GetComponent<Rigidbody>().AddForce(targetDir * 0.7f, ForceMode.Acceleration);
                
            }
        }
        
        
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

        
        // check if colliding with obstacle
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("collided with obstacle");
            // call split function
            ObstacleSplit();
        }

        else if (playerSize > other.transform.localScale.magnitude)
        {
            Debug.Log("collided with player");
            other.transform.position = Vector3.MoveTowards(other.transform.position, transform.position, 0.2f);
            other.gameObject.SetActive(false);
            playerSize += other.transform.localScale.magnitude * 0.2f;

        }
        // bot olma durumu kaldı 
        
        // transformunun küçük olması kaldı zaten o da game over
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
        
        /*
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, transform.position,
            Quaternion.identity);
        splitPiece.transform.localScale = playersizeVector;
        splitPiece.GetComponent<Rigidbody>().AddForce(Vector3.forward, ForceMode.Impulse);
        splitPiece.transform.parent = transform;
        splittedPieces.Add(splitPiece);
        */
        // update player size and spawned piece size as needed
        playerSize -= playerSize * 0.5f;
        Vector3 playersizeVector = new Vector3(playerSize, playerSize, playerSize);
        
        // calculate spawn position (forward from parent)
        Vector3 spawnPosition = transform.position + transform.position * 2f;
        
        // spawn the split object
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, transform.position + transform.forward,
            Quaternion.identity);
        splitPiece.transform.localScale = playersizeVector;
        // set the split object as a child of the parent 
        splittedPieces.Add(splitPiece);
        
    }


    public void ObstacleSplit()
    {
        // belli bi boyuttan küçükse oyunu bitir 

        if (playerSize < 1f)
        {
            Debug.Log("your size is less than 1, GAME OVER");
        }
        
        playerSize -= playerSize * .5f;
        int numSplitParts = 4;
        
        for (int i = 0; i < numSplitParts; i++)
        {
            float angle = 360f * i / numSplitParts;
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * splitRadius;
            Vector3 spawnPosition = transform.position + offset;
            NetworkObject splitPart = Runner.Spawn(splittedPiecePref, spawnPosition, Quaternion.identity);
            splitPart.transform.localScale = new Vector3(playerSize, playerSize, playerSize);
            Rigidbody rigid = splitPart.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                
                Vector3 forceDirection = (splitPart.transform.position - transform.position).normalized;
                rigid.AddForce(forceDirection * 2f, ForceMode.Impulse);
            }
            splittedPieces.Add(splitPart);
        }
       

    }
    /*
    private void MoveSplitCellsTogether()
    {
        if (splittedPieces.Count > 0)
        {
            Vector3 centerPosition = transform.position;

            foreach (NetworkObject piece in splittedPieces)
            {
                centerPosition += piece.transform.position;
            }

            centerPosition /= splittedPieces.Count;

            foreach (NetworkObject cell in splittedPieces)
            {
                Rigidbody rb = cell.GetComponent<Rigidbody>();
                Vector3 moveDirection = (centerPosition - cell.transform.position).normalized;
                rb.AddForce(moveDirection  * 0.5f, ForceMode.Impulse);
            }
        }
    }
    */
  
}