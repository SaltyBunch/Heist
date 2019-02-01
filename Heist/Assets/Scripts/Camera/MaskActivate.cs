using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskActivate : MonoBehaviour
{
    public float speed;
    [SerializeField] private Material mat;

    public void StartShow()
    {
        StopAllCoroutines();
        var temp = mat.color;
        if (temp.a < 0.1f)
        {
            temp = new Color(temp.r, temp.g, temp.b, 0.2f);
            mat.color = temp;
        }
        StartCoroutine(Activate());
    }

    public void StopShow()
    {
        StopAllCoroutines();
        StartCoroutine(Deactivate());
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

    IEnumerator Deactivate()
    {
        var temp = mat.color;
        while (mat.color.a > 0)
        {
            temp = new Color(temp.r, temp.g, temp.b, temp.a - 0.01F);
            mat.color = temp;
            yield return new WaitForSeconds(speed*1.5f);
        }
    }
}
