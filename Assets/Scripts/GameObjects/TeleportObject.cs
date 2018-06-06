using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportObject : MonoBehaviour
{
    public GameObject connectedObject;

    private Vector3 connectedObjectPosition;

    // Use this for initialization
    void Start ()
    {
        if (connectedObject != null)
        {
            connectedObjectPosition = connectedObject.transform.position;
        }
        else
        {
            Debug.Log("No Object Connected!");
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        ITeleportable teleportable = other.gameObject.GetComponent<ITeleportable>();
        if (teleportable != null)
        {
            teleportable.Teleport(connectedObjectPosition);
        }
        else
        {
            Debug.Log("No ITeleportable!");
        }

    }



}
