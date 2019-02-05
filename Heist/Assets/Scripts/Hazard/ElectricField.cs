using System.Collections;
using Character;
using Game;
using UnityEngine;

namespace Hazard
{
    public class ElectricField : Hazard
    {
        private bool[] _players;

        [SerializeField] public BoxCollider Collider;
        [SerializeField] public GameObject Electric1;
        [SerializeField] public GameObject Electric2;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
            _players = new bool[GameManager.PlayerChoice.Length];
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                StartCoroutine(Trigger(other.GetComponentInParent<PlayerControl>()));
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _players[other.GetComponent<PlayerControl>().PlayerNumber - 1] = false;
            }
        }    

        private new IEnumerator Trigger(PlayerControl player)
        {
            var prevSpeed = player.BaseCharacter.Stats.Speed;
            _players[player.PlayerNumber - 1] = true;
            player.BaseCharacter.Stats.Speed /= 2;
            do
            {
                player.BaseCharacter.Stacks += 1;
                yield return new WaitForSeconds(1f);
            } while (_players[player.PlayerNumber - 1]);
            player.BaseCharacter.Stats.Speed = prevSpeed;
        }
    }
}