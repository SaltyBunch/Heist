using Character;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(Collider))]
    public class LevelTeleport : MonoBehaviour
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _exit;

        private void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<PlayerControl>();
                player.transform.position = _exit.position;
                player.CameraLogic.transform.position = _exit.position;
            }
        }
    }
}