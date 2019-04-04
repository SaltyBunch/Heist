using Game;
using UnityEngine;

public class LightChange : MonoBehaviour
{
    private bool f = true;
    [SerializeField] private Light light;

    private void Update()
    {
        if (LevelManager.LevelManagerRef.vaultOpen && f)
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