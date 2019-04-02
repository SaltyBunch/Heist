using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChange : MonoBehaviour
{
    [SerializeField] Light light;
    bool f = true;

    private void Update()
    {
        if (Game.LevelManager.LevelManagerRef.vaultOpen && f)
        {
            ChangeLightToRed();
            f = false;
        }
    }

    public void ChangeLightToRed()
    {
        light.color = Color.red;
    }
}
