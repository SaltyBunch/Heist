using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private List<GameObject> playerImages;
    [SerializeField] private List<Text> playerNames;

    public void Initialize(List<string> PN, List<GameObject> PI)
    {
        for (var i = 0; i < PN.Count; i++)
        {
            playerNames[i].text = PN[i];
            playerImages[i] = PI[i];
        }

        for (var i = PN.Count; i < 4; i++)
        {
            Destroy(playerNames[i]);
            Destroy(playerImages[i]);
        }
    }
}