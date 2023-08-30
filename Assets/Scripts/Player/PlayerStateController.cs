
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class PlayerStateController : NetworkBehaviour
{
    //public bool isBot;
    
    // color change related networked properties
    public MeshRenderer MeshRenderer;
    
    [Networked(OnChanged = nameof(NetworkColorChanged))]
    public Color NetworkedColor { get; set; }

    public static PlayerStateController StateInstance;

    
    // size and score change related networked properties
    [Networked(OnChanged = nameof(NetworkSizeChanged))]
    public float NetworkedSize { get; set; } = 0f;


    public Text playerScoretext { get; set; }
    
        
    //[Networked(OnChanged = nameof(NetworkScoreChanged))]
    public int playerScore { get; set; }
    

    // split controls
    private bool splitPressed;
    public float splitRadius = 2f;
    private float splitThreshold = 2f;
    public List<NetworkObject> splittedPieces = new List<NetworkObject>();
    public GameObject splittedPiecePref;
    

    
    // main
    private Rigidbody rb;
    public Transform InterpolationObj;


    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        InterpolationObj = GetComponentInChildren<Transform>();
        MeshRenderer = GetComponent<MeshRenderer>();
        playerScoretext = transform.parent.GetChild(3).GetChild(1).GetComponentInChildren<Text>();
       
        if (StateInstance == null)
        {
            StateInstance = this;
        }
    }

    public override void Spawned()
    {
        playerScore = 0;
        // set random color
        NetworkedColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1f);
    }
    

    public override void FixedUpdateNetwork()
    {
        // to make sure that spawned parts will follow the main player
        /*
        if (splittedPieces.Count > 0)
        {
            foreach (NetworkObject piece in splittedPieces.ToArray())
            {
                // Check if the piece is still in the list
                if (splittedPieces.Contains(piece))
                {
                    Rigidbody pieceRigidbody = piece.GetComponent<Rigidbody>();
                    if (pieceRigidbody != null)
                    {
                        Vector3 targetDir = InterpolationObj.position - piece.transform.position;
                        pieceRigidbody.AddForce(targetDir * 0.4f);
                    }
                }
            }
        
        }
            */
        
    }

    public void OnTriggerEnter(Collider collided)
    {
        if (collided.gameObject.CompareTag("Food"))
        {
            
            Debug.Log("collided with food");
            NetworkedSize += 0.07f;
            Debug.Log(NetworkedSize + "collided food");
            collided.gameObject.transform.position = Utils.GetRandomSpawnPosition(collided.transform.localScale.x);
        }
    }

    
    // COLLISION STATES
    public void OnCollisionEnter(Collision other)
    {
        // when collided with food change its size and update score and position of the food
        
        
        
        // check if colliding with obstacle
        if (other.gameObject.CompareTag("Obstacle"))
        {
            Debug.Log("collided with obstacle");
            // call split function
            if (NetworkedSize > 1f)
            {
                ObstacleSplit();
            }
            else
            {
                NetworkedSize = 0.4f;
                Debug.Log("your size is less than 1.5, reduce ");
            }
        }

        // other collision checks for  player pieces, other players and bot players
        if (other.gameObject.CompareTag("SplittedPiece"))
        {
            Debug.Log("collided with playerpiece");
  
            NetworkedSize += other.transform.localScale.x;
            Debug.Log(NetworkedSize);
            splittedPieces.Remove(other.transform.GetComponent<NetworkObject>()); 
            Runner.Despawn(other.transform.GetComponent<NetworkObject>());

            
            // to do :  botsa ve benden büyükse game over, botsa ve benden küçükse ben yedim
            // ben botsam ama input auhtorityliyi yersem onun oyunu bitti o beni yerse devam

        }

        else if(other.gameObject.CompareTag("Bot"))
        {
            Debug.Log("collided with bot");
            if (other.transform.localScale.x > NetworkedSize)
            {
                Debug.Log("bot is bigger than you");
            }
            
            Debug.Log("player is bigger than bot");
            other.gameObject.transform.position = Utils.GetRandomSpawnPosition(other.transform.localScale.x);
                
        }

        // Game over if its size is smaller 
    }

    
    
    // Change functions for Networked Properties
    
    // since score will be related with size, they can be managed with one Change action
    private static void NetworkSizeChanged(Changed<PlayerStateController> changed)
    {

        Vector3 newScale = Vector3.one * changed.Behaviour.NetworkedSize;
        changed.Behaviour.rb.transform.localScale = newScale;
        Debug.Log("playerSize" + changed.Behaviour.rb.transform.localScale);

        //float playerScore = 0f;
        int playerScore =  Mathf.RoundToInt(changed.Behaviour.NetworkedSize * 20f);
        changed.Behaviour.playerScore = playerScore;
        changed.Behaviour.playerScoretext.text = "SCORE : " + playerScore;
        Debug.Log("playerscore int" + changed.Behaviour.playerScore);
        Debug.Log("playerscore float" + changed.Behaviour.NetworkedSize * 20f);
        Debug.Log("Networked playerscoretext" + changed.Behaviour.playerScoretext.text);
        Debug.Log("playerscoretext" + changed.Behaviour.playerScore);
    }

    
    
    
    
    // color change when spawned, splietted pieces should be spawned with the same color
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
    
    
    // space split is only possible if the playerSize is bigger than threshold 
    // for space split since it is intentional add the scores collected by the piece
    public void SpaceSplit()
    {
        // update player size and spawned piece size as needed
        NetworkedSize *=0.5f;
        Vector3 playersizeVector = new Vector3(NetworkedSize, NetworkedSize, NetworkedSize);
        
        float angle = 360f;
        Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * splitRadius;
        Vector3 spawnPosition = transform.position + offset;
        // calculate spawn position (forward from parent)
        // spawn the split object
        NetworkObject splitPiece = Runner.Spawn(splittedPiecePref, spawnPosition,
            Quaternion.identity,null, InitializeBeforeSplitPartSpawn);
        //splitPiece.GetComponent<MeshRenderer>().material.color = NetworkedColor;
        splitPiece.transform.localScale = playersizeVector;
        //splitPiece.transform.parent = transform;
        Rigidbody rigid = splitPiece.GetComponent<Rigidbody>();
        
        // move pieces a bit to prevent collision at start
        
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
        
        int numSplitParts = 3;
        NetworkedSize /= numSplitParts;

        
        for (int i = 0; i < numSplitParts; i++)
        {
            float angle = 360f * i / numSplitParts;
            Vector3 offset = Quaternion.Euler(0f, angle, 0f) * Vector3.forward * splitRadius;
            Vector3 spawnPosition = transform.position + offset;
            NetworkObject splitPart = Runner.Spawn(splittedPiecePref, spawnPosition, Quaternion.identity,null, InitializeBeforeSplitPartSpawn);
            //splitPart.GetComponent<MeshRenderer>().material.color = NetworkedColor;
            splitPart.transform.localScale = new Vector3(NetworkedSize, NetworkedSize, NetworkedSize);
            splittedPieces.Add(splitPart);
            
            // you might face with problems if you add a network object as a child of other network obj
            //splitPart.transform.parent = transform;

            Rigidbody rigid = splitPart.GetComponent<Rigidbody>();
            if (rigid != null)
            {
                Vector3 forceDirection = (splitPart.transform.position - transform.position).normalized;
                rigid.AddForce(forceDirection , ForceMode.Impulse);
            }

        }
    }
    public void InitializeBeforeSplitPartSpawn(NetworkRunner runner, NetworkObject networkObject)
    {
        networkObject.GetComponent<MeshRenderer>().material.color = NetworkedColor;
        networkObject.GetComponent<SpawnedCollisionObstacle>().SetSpawner(this);
    }
    
    
  
}