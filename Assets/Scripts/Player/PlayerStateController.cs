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
    public MeshRenderer MeshRenderer;

    // networked properties
    [Networked(OnChanged = nameof(NetworkSizeChanged))]
    public float playerSize { get; set; }
    
    [Networked(OnChanged = nameof(PlayerScoreChanged))]
    public float playerScore { get; set; }
    
    [Networked(OnChanged = nameof(NetworkColorChanged))]
    public Color NetworkedColor { get; set; }
    
    
    private Rigidbody rb;
    private bool splitPressed;
    public float splitRadius = 2f;
    private float splitThreshold = 2f;

    public GameObject splittedPiecePref;
    [SerializeField] private LayerMask foodLayerMask;
    [SerializeField] List<GameObject> foodTarget;
    [SerializeField] Collider[] hitcolliders;
    public List<NetworkObject> splittedPieces = new List<NetworkObject>();
    [SerializeField] private Transform InterpolationObj;


    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InterpolationObj = GetComponentInChildren<Transform>();
        MeshRenderer = GetComponent<MeshRenderer>();

    }

    // size senin kendi objende değişiyo ama networkte göstermelisin networked
    // property olmalı

    public override void Spawned()
    {
        playerScore = 0;
        // Set a color to player when spawned
        NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1f);
    }
    

    public override void FixedUpdateNetwork()
    {
        playerScore = playerSize * 20;
        
        if (splittedPieces.Count > 0)
        {
            foreach (NetworkObject piece in splittedPieces)
            {
                Vector3 targetDir = InterpolationObj.position  - piece.transform.position;
                piece.GetComponent<Rigidbody>().AddForce(targetDir* 2f);
                
            }
        }
        Debug.Log("playerSize" + playerSize);
        Debug.Log("playerScore" + playerScore);

    }


    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("collided with food");
            playerSize += playerSize * 0.2f;
            rb.transform.localScale = new Vector3(playerSize, playerSize, playerSize);
            other.gameObject.transform.position = Utils.GetRandomSpawnPosition(other.transform.localScale.x);
        }

        
        // check if colliding with obstacle
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("collided with obstacle");
            // call split function
            if (playerSize > 1f)
            {
                ObstacleSplit();

            }
            else
                Debug.Log("your size is less than 1, GAME OVER");
        }

        else if (playerSize > other.transform.localScale.magnitude)
        {
            Debug.Log("collided with player");
            other.transform.position = Vector3.MoveTowards(other.transform.position, transform.position, 0.2f);
            other.gameObject.SetActive(false);
            playerSize += other.transform.localScale.magnitude * 0.2f;

        }

        // transformunun küçük olması kaldı zaten o da game over
    }

    private static void NetworkSizeChanged(Changed<PlayerStateController> changed)
    {
        changed.Behaviour.playerSize = changed.Behaviour.playerSize;
        
    }
    private static void PlayerScoreChanged(Changed<PlayerStateController> changed)
    {
        changed.Behaviour.playerScore = changed.Behaviour.playerScore;

    }

    private static void NetworkColorChanged(Changed<PlayerStateController> changed)
    {
        changed.Behaviour.MeshRenderer.material.color = changed.Behaviour.NetworkedColor;
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
        
        // update player size and spawned piece size as needed
        playerSize -= playerSize * 0.5f;
        Vector3 playersizeVector = new Vector3(playerSize, playerSize, playerSize);
        
        // calculate spawn position (forward from parent)
        Vector3 spawnPosition = transform.position + transform.position * 2f;
        
        // spawn the split object
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, spawnPosition,
            Quaternion.identity);
        splitPiece.GetComponent<MeshRenderer>().material.color = NetworkedColor;
        splitPiece.transform.localScale = playersizeVector;
        // set the split object as a child of the parent 
        splittedPieces.Add(splitPiece);
        
    }


    public void ObstacleSplit()
    {
        playerSize -= playerSize * .5f;
        int numSplitParts = 4;
        
        for (int i = 0; i < numSplitParts; i++)
        {
            float angle = 360f * i / numSplitParts;
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * splitRadius;
            Vector3 spawnPosition = transform.position + offset;
            NetworkObject splitPart = Runner.Spawn(splittedPiecePref, spawnPosition, Quaternion.identity);
            splitPart.GetComponent<MeshRenderer>().material.color = NetworkedColor;
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
    

    
    
  
}