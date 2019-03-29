using System.Collections;
using Character;
using Game;
using UnityEngine;

namespace Hazard
{
    public class LethalLaser : Hazard
    {
        [SerializeField] [Range(0.5f, 10)] private float _cooldown;


        public BoxCollider Collider;
        public GameObject Laser1;
        public GameObject Laser2;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
        }

        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                LevelManager.LevelManagerRef.Notify(other.transform.position, NotifyType.TripTrap);
                var player = other.GetComponentInParent<PlayerControl>();
                player.BaseCharacter.Stacks += 2;
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<PlayerControl>();
                player.BaseCharacter.Stacks += 2;
            }
        }

        public override bool Place(Vector3 position)
        {
            LayerMask layers = (LevelManager.LevelManagerRef.EnvironmentLayer);

            Vector3 fwd = Vector3.positiveInfinity,
                rt = Vector3.positiveInfinity,
                lt = Vector3.negativeInfinity,
                bk = Vector3.negativeInfinity;

            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.forward, out hit, _maxGap, layers)) fwd = hit.point;

            if (Physics.Raycast(position, Vector3.right, out hit, _maxGap, layers)) rt = hit.point;

            if (Physics.Raycast(position, Vector3.left, out hit, _maxGap, layers)) lt = hit.point;

            if (Physics.Raycast(position, Vector3.back, out hit, _maxGap, layers)) bk = hit.point;

            if (Vector3.Distance(fwd, bk) > _maxGap && Vector3.Distance(rt, lt) > _maxGap) return false;
            
            if (Vector3.Distance(fwd, bk) > Vector3.Distance(rt, lt))
            {
                transform.position = (rt + lt) / 2;

                transform.LookAt(rt);

                Laser1.transform.position = rt + Vector3.up;
                Laser2.transform.position = lt + Vector3.up;


                Laser1.transform.LookAt(Laser2.transform);
                Laser2.transform.LookAt(Laser1.transform);


                Collider.size = new Vector3
                {
                    x = Laser1.transform.localPosition.x * 2 + 0.1f,
                    z = Laser1.transform.localPosition.z * 2, y = 1.5f
                };
            }
            else
            {
                //vertical is less than horizontal
                transform.position = (fwd + bk) / 2;

                transform.LookAt(fwd);

                Laser1.transform.position = fwd + Vector3.up;
                Laser2.transform.position = bk + Vector3.up;


                Laser1.transform.LookAt(Laser2.transform);
                Laser2.transform.LookAt(Laser1.transform);


                Collider.size = new Vector3
                {
                    x = Laser1.transform.localPosition.x * 2 + 0.1f,
                    z = Laser1.transform.localPosition.z * 2, y = 1.5f
                };
            }

            return true;
        }
    }
}