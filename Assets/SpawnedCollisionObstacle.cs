using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.StructWrapping;
using Fusion;
using UnityEngine;

public class SpawnedCollisionObstacle : NetworkBehaviour
{
    
    public void OnCollisionEnter(Collision other)
    {
        if (other.transform.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }
    }
    
}
