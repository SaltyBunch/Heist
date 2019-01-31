using System.Collections;
using Character;
using Game;
using UnityEngine;
using UnityEngine.Analytics;

namespace Hazard
{
    public class ElectricField : Hazard
    {
        private bool[] _players;

        private void Awake()
        {
            _players = new bool[GameManager.PlayerChoice.Length];
        }

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
                _players[other.GetComponent<PlayerControl>().PlayerNumber - 1] = false;
            }
        }

        private new IEnumerator Trigger(PlayerControl player)
        {
            var prevSpeed = player.BaseCharacer.Stats.Speed;
            _players[player.PlayerNumber - 1] = true;
            player.BaseCharacer.Stats.Speed = 2;
            do
            {
                player.BaseCharacer.Stacks += 1;
                yield return new WaitForSeconds(1f);
            } while (_players[player.PlayerNumber - 1]);

            player.BaseCharacer.Stats.Speed = prevSpeed;
        }
    }
}