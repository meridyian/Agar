using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;

public class BotController : NetworkBehaviour
{
    // food collection related 
    [SerializeField] private LayerMask foodLayerMask;
    [SerializeField] List<NetworkObject> foodTargets;
    [SerializeField] Collider[] hitcolliders;
    private NetworkObject targetFood;
    private Rigidbody botBody;


    
    public override void Spawned()
    {
        botBody = GetComponent<Rigidbody>();
        
    }

    public override void FixedUpdateNetwork()
    {
        foodTargets = new List<NetworkObject>();
        if (targetFood == null)
        {
            hitcolliders = new Collider[10];
            Runner.GetPhysicsScene().OverlapSphere(transform.position, 15f, hitcolliders, foodLayerMask, QueryTriggerInteraction.UseGlobal);
            foreach (Collider col in hitcolliders)
            {
                if(col.CompareTag("Food"))
                    foodTargets.Add(col.GetComponent<NetworkObject>());
            }
            
            targetFood = foodTargets[Random.Range(0, foodTargets.Count)];
        }
        else
        {
            Vector3 moveTarget = (targetFood.transform.position - transform.position).normalized;
            botBody.AddForce(moveTarget * transform.localScale.magnitude, ForceMode.Acceleration);
        }

    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Food"))
        {
            
            other.transform.position = Utils.GetRandomSpawnPosition(other.transform.localScale.x);
            
            if (transform.localScale.x < 4f)
            {
                transform.localScale += Vector3.one*0.5f;
            }
        }

        if (other.gameObject.CompareTag("Obstacle"))
        {
            transform.localScale = Vector3.one * 1.5f;
            transform.position = Utils.GetRandomSpawnPosition(transform.localScale.x);
        }
    }
}
