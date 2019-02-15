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
            if (other.CompareTag("Player")) StartCoroutine(Trigger(other.GetComponentInParent<PlayerControl>()));
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) _players[other.GetComponent<PlayerControl>().PlayerNumber - 1] = false;
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

        public override void Place(Vector3 position)
        {
            var dis = 1.24f;
            LayerMask layers = ~LayerMask.GetMask("Hazard", "Environment", "VFX");

            Vector3 fwd = Vector3.positiveInfinity,
                rt = Vector3.positiveInfinity,
                lt = Vector3.positiveInfinity,
                bk = Vector3.positiveInfinity;

            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.forward, out hit, 20, layers)) fwd = hit.point;

            if (Physics.Raycast(position, Vector3.right, out hit, 20, layers)) rt = hit.point;

            if (Physics.Raycast(position, Vector3.left, out hit, 20, layers)) lt = hit.point;

            if (Physics.Raycast(position, Vector3.back, out hit, 20, layers)) bk = hit.point;

            if (Vector3.Distance(fwd, bk) > Vector3.Distance(rt, lt))
            {
                transform.position = (rt + lt) / 2;

                transform.LookAt(rt);

                Electric1.transform.position = rt + Vector3.left * dis;
                Electric2.transform.position = lt + Vector3.right * dis;


                Electric1.transform.LookAt(Electric2.transform);
                Electric2.transform.LookAt(Electric1.transform);


                Collider.size = new Vector3
                {
                    x = Electric1.transform.localPosition.x * 2 + 0.1f,
                    z = Electric1.transform.localPosition.z * 2, y = 1.5f
                };
            }
            else
            {
                //vertical is less than horizontal
                transform.position = (fwd + bk) / 2;

                transform.LookAt(fwd);

                Electric1.transform.position = fwd + Vector3.back * dis;
                Electric2.transform.position = bk + Vector3.forward * dis;


                Electric1.transform.LookAt(Electric2.transform);
                Electric2.transform.LookAt(Electric1.transform);


                Collider.size = new Vector3
                {
                    x = Electric1.transform.localPosition.x * 2 + 0.1f,
                    z = Electric1.transform.localPosition.z * 2, y = 1.5f
                };
            }
        }
    }
}