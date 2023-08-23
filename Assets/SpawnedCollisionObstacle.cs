using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnedCollisionObstacle : NetworkBehaviour
{

    [SerializeField] private Rigidbody splittedPartRb;



    public override void Spawned()
    {
        splittedPartRb = GetComponent<Rigidbody>();
    }

    
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            transform.parent.GetComponent<PlayerStateController>().splittedPieces.Remove(this.GetComponent<NetworkObject>());
        }
        
        if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("Splitted piece collided with food");
            transform.parent.GetComponent<PlayerStateController>().NetworkedSize += other.transform.localScale.magnitude * 0.3f;
        }
        /*
        
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Splitted piece collided with player");
            Destroy(gameObject);
            transform.parent.GetComponent<PlayerStateController>().splittedPieces.Remove(this.GetComponent<NetworkObject>());
            transform.parent.GetComponent<PlayerStateController>().playerSize += other.transform.localScale.magnitude;

        }
        */
    }
   
    
}
