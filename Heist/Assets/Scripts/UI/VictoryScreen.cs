using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] List<Text> playerNames;
    [SerializeField] List<GameObject> playerImages;

    public void Initialize(List<string> PN, List<GameObject> PI)
    {
        for (int i = 0; i < PN.Count; i++)
        {
            playerNames[i].text = PN[i];
            playerImages[i] = PI[i];
        }
        for (int i = PN.Count; i < 4; i++)
        {
            Destroy(playerNames[i]);
            Destroy(playerImages[i]);
        }
    }



}
