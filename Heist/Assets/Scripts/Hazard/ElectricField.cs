using Character;
using Game;
using UnityEngine;

namespace Hazard
{
    public class ElectricField : Hazard
    {
        private readonly RaycastHit[] _colliders = new RaycastHit[5];
        [SerializeField] public BoxCollider Collider;
        [SerializeField] public GameObject Electric1;
        [SerializeField] public GameObject Electric2;

        private void Awake()
        {
            Collider = GetComponent<BoxCollider>();
        }

        private void OnTriggerEnter(Collider other)
        {
            LevelManager.LevelManagerRef.Notify(other.transform.position, NotifyType.TripTrap);
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<PlayerControl>();
                player.BaseCharacter.Stats.Speed -= 2;
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

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<PlayerControl>();
                player.BaseCharacter.Stats.Speed += 2;
            }
        }

        public override bool Place(Vector3 position)
        {
            var dis = 1.24f;
            int layers = LevelManager.LevelManagerRef.EnvironmentLayer;

            Vector3 fwd = Vector3.positiveInfinity,
                rt = Vector3.positiveInfinity,
                lt = Vector3.negativeInfinity,
                bk = Vector3.negativeInfinity;

            //forward
            var size = Physics.RaycastNonAlloc(position, Vector3.forward, _colliders, _maxGap, layers);
            for (var i = 0; i < size; i++)
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    fwd = _colliders[i].point;
                    break;
                }

            //right
            size = Physics.RaycastNonAlloc(position, Vector3.right, _colliders, _maxGap, layers);
            for (var i = 0; i < size; i++)
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    rt = _colliders[i].point;
                    break;
                }

            //left
            size = Physics.RaycastNonAlloc(position, Vector3.left, _colliders, _maxGap, layers);
            for (var i = 0; i < size; i++)
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    lt = _colliders[i].point;
                    break;
                }

            //back
            size = Physics.RaycastNonAlloc(position, Vector3.back, _colliders, _maxGap, layers);
            for (var i = 0; i < size; i++)
                if (_colliders[i].transform.GetComponent<Rigidbody>() == null)
                {
                    bk = _colliders[i].point;
                    break;
                }

            if (Vector3.Distance(fwd, bk) > _maxGap && Vector3.Distance(rt, lt) > _maxGap) return false;

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
                    z = Electric1.transform.localPosition.z * 2,
                    y = 1.5f
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
                    z = Electric1.transform.localPosition.z * 2,
                    y = 1.5f
                };
            }

            return true;
        }
    }
}