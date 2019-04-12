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
        private RaycastHit[] _colliders = new RaycastHit[5];

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<Character.Character>();
                if (_placedBy != player)
                {
                    player.Stacks += Damage;
                    LevelManager.LevelManagerRef.Notify(other.transform.position, NotifyType.TripTrap);
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<Character.Character>();
                if (_placedBy != player)
                {
                    player.Stacks += Damage;
                }
            }
        }

        public override bool Place(Vector3 position, Character.Character player)
        {
            int layers = (LevelManager.LevelManagerRef.EnvironmentLayer);

            Vector3 fwd = Vector3.positiveInfinity,
                rt = Vector3.positiveInfinity,
                lt = Vector3.negativeInfinity,
                bk = Vector3.negativeInfinity;

            //forward
            var size = Physics.RaycastNonAlloc(position, Vector3.forward, _colliders, _maxGap, layers);
            for (int i = 0; i < size; i++)
            {
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    fwd = _colliders[i].point;
                    break;
                }
            }

            //right
            size = Physics.RaycastNonAlloc(position, Vector3.right, _colliders, _maxGap, layers);
            for (int i = 0; i < size; i++)
            {
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    rt = _colliders[i].point;
                    break;
                }
            }

            //left
            size = Physics.RaycastNonAlloc(position, Vector3.left, _colliders, _maxGap, layers);
            for (int i = 0; i < size; i++)
            {
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    lt = _colliders[i].point;
                    break;
                }
            }

            //back
            size = Physics.RaycastNonAlloc(position, Vector3.back, _colliders, _maxGap, layers);
            for (int i = 0; i < size; i++)
            {
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    bk = _colliders[i].point;
                    break;
                }
            }
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
                    z = Laser1.transform.localPosition.z * 2,
                    y = 1.5f
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
                    z = Laser1.transform.localPosition.z * 2,
                    y = 1.5f
                };
            }

            _placedBy = player;

            StartCoroutine(RemovePlayer());

            return true;
        }
    }
}