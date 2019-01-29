using System.Collections;
using Character;
using UnityEngine;

namespace Hazard
{
    [RequireComponent(typeof(LineRenderer))]
    public class LethalLaser : Hazard
    {
        public GameObject Laser1;
        public GameObject Laser2;

        public LineRenderer[] LineRenderer;


        public BoxCollider Collider;

        [SerializeField, Range(0.5f, 10)] private float _cooldown;

        private void Awake()
        {
            LineRenderer = GetComponentsInChildren<LineRenderer>();
            Collider = GetComponent<BoxCollider>();
        }

        private void Reset()
        {
            if (LineRenderer == null)
                LineRenderer = GetComponentsInChildren<LineRenderer>();
            foreach (var lr in LineRenderer)
            {
                lr.startWidth = 0.1f;
                lr.endWidth = 0.1f;
            }
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
                //todo actual trap logic
                yield return null;
                elapsed += Time.deltaTime;
            } while (elapsed < _cooldown);

            foreach (var lr in LineRenderer)
            {
                lr.enabled = true;
            }
        }
    }
}