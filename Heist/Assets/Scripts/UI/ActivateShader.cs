using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateShader : MonoBehaviour
{
    [SerializeField] private Shader show;
    [SerializeField] private Shader standard;
    [SerializeField] string tagName;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == tagName)
        {
            foreach (var v in other.GetComponentsInChildren<MeshRenderer>())
            {
                v.material.shader = show;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == tagName)
        {
            foreach (var v in other.GetComponentsInChildren<MeshRenderer>())
            {
                v.material.shader = standard;
            }
        }
    }
}
