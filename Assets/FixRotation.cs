using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class FixRotation : NetworkBehaviour
{
    /*
    // Start is called before the first frame update
    Vector3 rotation;
    private Transform emptyParent;


    void Awake()
    {
        emptyParent = transform.parent;
        rotation = transform.GetComponent<RectTransform>().eulerAngles;
    }

    public override void FixedUpdateNetwork()
    {
        rotation = Vector3.zero;
        transform.position = new Vector3(emptyParent.position.x , emptyParent.position.y + PlayerStateController.StateInstance.NetworkedSize, emptyParent.position.z );
    }
    */
}
