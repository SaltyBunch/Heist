using System.Collections;
using UnityEngine;

public class MaskActivate : MonoBehaviour
{
    [SerializeField] private Material mat;
    public float speed;

    public void StartShow()
    {
        gameObject.SetActive(true);
        //        StopAllCoroutines();
        //       var temp = mat.color;
        //      if (temp.a < 0.1f)
        //       {
        //           temp = new Color(temp.r, temp.g, temp.b, 0.1f);
        //           mat.color = temp;
        //       }
        //      StartCoroutine(Activate());
    }

    public void StopShow()
    {
        gameObject.SetActive(false);
        //StopAllCoroutines();
        //StartCoroutine(Deactivate());
    }

    private IEnumerator Activate()
    {
        var temp = mat.color;
        while (mat.color.a < 1)
        {
            temp = new Color(temp.r, temp.g, temp.b, temp.a + 0.01F);
            mat.color = temp;
            yield return new WaitForSeconds(speed);
        }
    }

    private IEnumerator Deactivate()
    {
        var temp = mat.color;
        while (mat.color.a > 0)
        {
            temp = new Color(temp.r, temp.g, temp.b, temp.a - 0.02F);
            mat.color = temp;
            yield return new WaitForSeconds(speed / 15f);
        }
    }
}