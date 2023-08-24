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
            PlayerStateController.StateInstance.splittedPieces.Remove(this.GetComponent<NetworkObject>());
            Runner.Despawn(transform.GetComponent<NetworkObject>());
           
        }
        
        if (other.gameObject.CompareTag("Food"))
        {
            Debug.Log("Splitted piece collided with food");
            PlayerStateController.StateInstance.NetworkedSize +=  0.2f;
            Runner.Despawn(transform.GetComponent<NetworkObject>());
            
            
        }
       
    }
   
    
}
