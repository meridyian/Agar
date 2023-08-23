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
    public float NetworkedSize { get; set; } = 1.0f;
    
    //[Networked(OnChanged = nameof(PlayerScoreChanged))]
    public float playerScore;
    
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

    public override void Spawned()
    {
        playerScore = 0;
        NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1f);
    }
    

    public override void FixedUpdateNetwork()
    {
        //playerScore = playerSize * 20;
        
        if (splittedPieces.Count > 0)
        {
            foreach (NetworkObject piece in splittedPieces)
            {
                Vector3 targetDir = InterpolationObj.position  - piece.transform.position;
                piece.GetComponent<Rigidbody>().AddForce(targetDir * 0.4f);
                
            }
        }
        Debug.Log("playerSize" + NetworkedSize);


    }

    
    // COLLISION STATES
    public void OnCollisionEnter(Collision other)
    {
        // when collided with food change its size and update score
        if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("collided with food");
            NetworkedSize += NetworkedSize * 0.2f;
            other.gameObject.transform.position = Utils.GetRandomSpawnPosition(other.transform.localScale.x);
        }
        
        // check if colliding with obstacle
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("collided with obstacle");
            // call split function
            if (NetworkedSize > 1.5f)
            {
                ObstacleSplit();
            }
            else
                Debug.Log("your size is less than 1, GAME OVER");
        }

        // other collision checks for  player pieces, other players and bot players
        else if (NetworkedSize > other.transform.localScale.x)
        {
            Debug.Log("collided with playerpiece or bot ");
  
            
            // check if it was a splitted piece
            
            if (other.transform.CompareTag("Player"))
            {
                NetworkedSize +=  0.3f;
                splittedPieces.Remove(other.transform.GetComponent<NetworkObject>()); 
                Destroy(other.gameObject);
                
            }
            
            // bıtsa başka yerde spawn edebilirsin
            //other.transform.position = Utils.GetRandomSpawnPosition(other.transform.localScale.x);
            //playerSize += other.transform.localScale.magnitude * 0.5f;

        }

        // Game over if its size is smaller 
    }

    
    
    // Change functions for Networked Properties
    private static void NetworkSizeChanged(Changed<PlayerStateController> changed)
    {
        Vector3 newScale = Vector3.one * changed.Behaviour.NetworkedSize;
        changed.Behaviour.rb.transform.localScale = newScale;
        
        float playerScore = changed.Behaviour.NetworkedSize * 20f;
        changed.Behaviour.playerScore = playerScore;
        Debug.Log("changed score" + changed.Behaviour.playerScore);

    }

    private static void NetworkColorChanged(Changed<PlayerStateController> changed)
    {
        changed.Behaviour.MeshRenderer.material.color = changed.Behaviour.NetworkedColor;
    }


    // split function to be called by new input system
    public void OnSplit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (NetworkedSize > splitThreshold)
            {
                // YOU CAN SPAWN PLAYERBODY IN HERE
                SpaceSplit(); 
           
            }
            Debug.Log("you are not big enough");
        }
    }
    
    
    // space split is only possible if the playerSize is bigger than threshold set
    // for splace split since it is intentional add the scores collected by the piece
    public void SpaceSplit()
    {
        // update player size and spawned piece size as needed
        NetworkedSize -= NetworkedSize * 0.5f;
        Vector3 playersizeVector = new Vector3(NetworkedSize, NetworkedSize, NetworkedSize);
        
        // calculate spawn position (forward from parent)
        // spawn the split object
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, transform.position + Vector3.one,
            Quaternion.identity);
        splitPiece.GetComponent<MeshRenderer>().material.color = NetworkedColor;
        splitPiece.transform.localScale = playersizeVector;
        splitPiece.transform.parent = transform;
        Rigidbody rigid = splitPiece.GetComponent<Rigidbody>();
        if (rigid != null)
        {
            Vector3 forceDirection = (splitPiece.transform.position - transform.position).normalized;
            rigid.AddForce(forceDirection , ForceMode.Impulse);
        }
        // set the split object as a child of the parent 
        splittedPieces.Add(splitPiece);
        
    }


    // for obstacle split case, split into 4 pieces add them to your list 
    // make them your child so that you can destroy when collided with obstacle
    public void ObstacleSplit()
    {
        NetworkedSize = 1f;
        int numSplitParts = 4;
        
        for (int i = 0; i < numSplitParts; i++)
        {
            float angle = 360f * i / numSplitParts;
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * splitRadius;
            Vector3 spawnPosition = transform.position + offset;
            NetworkObject splitPart = Runner.Spawn(splittedPiecePref, spawnPosition, Quaternion.identity);
            splitPart.GetComponent<MeshRenderer>().material.color = NetworkedColor;
            //splitPart.transform.localScale = new Vector3(NetworkedSize, NetworkedSize, NetworkedSize);
            splitPart.transform.parent = transform;

            Rigidbody rigid = splitPart.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                Vector3 forceDirection = (splitPart.transform.position - transform.position).normalized;
                rigid.AddForce(forceDirection , ForceMode.Impulse);
            }
            splittedPieces.Add(splitPart);
        }
    }
    

    
    
  
}