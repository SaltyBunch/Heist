using System.Collections;
using Character;
using UnityEngine;
using UnityEngine.Analytics;

namespace Hazard
{
    public class ElectricField : Hazard
    {
        private bool[] players;


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("player"))
            {
                StartCoroutine(Trigger(other.GetComponent<PlayerControl>()));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("player"))
            {
                players[other.GetComponent<PlayerControl>().PlayerNumber - 1] = false;
            }
        }

        private new IEnumerator Trigger(PlayerControl player)
        {
            players[player.PlayerNumber - 1] = true;
            do
            {
                //todo actual trap logic
                yield return new WaitForSeconds(1f);
            } while (players[player.PlayerNumber - 1]);
        }
    }
}