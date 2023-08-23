using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public class SpawnedCollisionObstacle : NetworkBehaviour
{
    
    // this script is attached to spawned o
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
            transform.parent.GetComponent<PlayerStateController>().splittedPieces.Remove(this.GetComponent<NetworkObject>());
        }
    }
    
    
}
