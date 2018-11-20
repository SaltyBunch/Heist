using Character;
using UnityEngine;

namespace Pickup
{
    [RequireComponent(typeof(Collider))]
    public class Pickup : MonoBehaviour
    {
        public enum PickupType
        {
            Weapon,
            Trap,
            Gold
        }

        [SerializeField] internal PickupType _pickupType;

        private void Start()
        {
            var collider = GetComponent<Collider>();
            if (!collider.isTrigger)
            {
                collider.isTrigger = true;
                Debug.LogError("Collider on {this.name} is not set as a Trigger, temporarily set to Trigger");
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player>();
                player.OverPickup(_pickupType, true, this);
            }
        }

        private void OnTriggerLeave(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponent<Player>();
                player.OverPickup(_pickupType, false, this);
            }
        }
    }
}