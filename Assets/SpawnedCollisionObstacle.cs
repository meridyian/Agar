using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SpawnedCollisionObstacle : NetworkBehaviour
{
    
    
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
            transform.parent.GetComponent<PlayerStateController>().NetworkedSize +=  0.2f;
            Destroy(gameObject);
            transform.parent.GetComponent<PlayerStateController>().splittedPieces.Remove(this.GetComponent<NetworkObject>());
            
        }
       
    }
   
    
}
