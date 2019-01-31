using System.Collections;
using Character;
using UnityEngine;

namespace Hazard
{
    public class LethalLaser : Hazard
    {
        public GameObject Laser1;
        public GameObject Laser2;


        public BoxCollider Collider;

        [SerializeField, Range(0.5f, 10)] private float _cooldown;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("player"))
            {
                StartCoroutine(Trigger(other.GetComponent<PlayerControl>()));
            }
        }

        private new IEnumerator Trigger(PlayerControl player)
        {
            float elapsed = 0;
            do
            {
                player.BaseCharacer.Stacks += 1;
                yield return null;
                elapsed += Time.deltaTime;
            } while (elapsed < _cooldown);
            
        }
    }
}