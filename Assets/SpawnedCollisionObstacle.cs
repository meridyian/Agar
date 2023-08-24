using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnedCollisionObstacle : NetworkBehaviour
{
    private PlayerStateController spawner;
    private Rigidbody rb;
    

    public void SetSpawner(PlayerStateController spawner)
    {
        this.spawner = spawner;
    }

    public  void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }



    public override void FixedUpdateNetwork()
    {
        if (rb != null)
        {
            Vector3 targetDir = spawner.InterpolationObj.position - rb.transform.position;
            rb.AddForce(targetDir * 0.4f);
        }
    }
    
    
    
    public void OnCollisionEnter(Collision other)
    {

        if (other.transform.CompareTag("Obstacle"))
        {
            //PlayerStateController.StateInstance.splittedPieces.Remove(this.GetComponent<NetworkObject>());
            spawner.splittedPieces.Remove(GetComponent<NetworkObject>());
            Runner.Despawn(transform.GetComponent<NetworkObject>());
           
        }
        
        if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("Splitted piece collided with food");
            //PlayerStateController.StateInstance.NetworkedSize +=  0.2f;
            spawner.NetworkedSize += 0.2f;
            Runner.Despawn(transform.GetComponent<NetworkObject>());
            
        }

        /*
        //collsiion with bot
        if (other.transform.CompareTag("Bot"))
        {
            spawner.splittedPieces.Remove(GetComponent<NetworkObject>());
            spawner.NetworkedSize -= 0.1f;
            Runner.Despawn(transform.GetComponent<NetworkObject>());
        }
        */
       
    }

}
