using System.Collections;
using Character;
using UnityEngine;

namespace Pickup
{
    public enum PickupType
    {
        Weapon,
        Trap,
        Gold,
        Key
    }

    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        private int _direction = 1;


        internal int _ignorePlayer = -1;
        [SerializeField] internal PickupType _pickupType;
        private Coroutine _rotation;

        [SerializeField] private float _verticalSpeed = 0.02f, _rotationSpeed = 1;

        private void Start()
        {
            var collider = GetComponent<Collider>();
            if (!collider.isTrigger)
            {
                collider.isTrigger = true;
                Debug.LogError("Collider on {this.name} is not set as a Trigger, temporarily set to Trigger");
            }

            _rotation = StartCoroutine(Rotate());
        }

        private IEnumerator Rotate()
        {
            float current = 0;
            do
            {
                current += _verticalSpeed;
                if (current > 1)
                {
                    current = 0;
                    _direction *= -1;
                }

                transform.position = Vector3.Lerp(transform.position,
                    transform.position + Vector3.up * 0.5f * _direction, _verticalSpeed);
                transform.localRotation = transform.localRotation * Quaternion.Euler(0, _rotationSpeed, 0);
                yield return new WaitForFixedUpdate();
            } while (true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<Player>();
                if (player.PlayerControl.PlayerNumber != _ignorePlayer)
                    player.OverPickup(_pickupType, true, this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<Player>();
                if (player.PlayerControl.PlayerNumber != _ignorePlayer)
                    player.OverPickup(_pickupType, false, this);
            }
        }

        private void OnDestroy()
        {
            StopCoroutine(_rotation);
        }
    }
}