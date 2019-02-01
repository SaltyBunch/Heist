using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskActivate : MonoBehaviour
{
    public float speed;
    [SerializeField] private Material mat;


    private void OnEnable()
    {
        StartCoroutine(Activate());
    }
    private void OnDisable()
    {
        var temp = mat.color;
        temp = new Color(temp.r, temp.g, temp.b, 0.2f);
        mat.color = temp;
        StopCoroutine(Activate());
    }

    IEnumerator Activate()
    {
        var temp = mat.color;
        while (mat.color.a < 1)
        {
            temp = new Color(temp.r, temp.g, temp.b, temp.a + 0.01F);
            mat.color = temp;
            yield return new WaitForSeconds(speed);
        }


    }
}
