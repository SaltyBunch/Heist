using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextZone : MonoBehaviour
{
    public Transform exitPlace;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "Player")
        {
            //collision.transform.parent.position = exitPlace.position;
            collision.transform.position = exitPlace.position;
        }
    }
}
