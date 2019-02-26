using Game;
using UnityEngine;

namespace Pickup
{
    public class KeyPickup : Pickup
    {
        [SerializeField] public KeyType Key;

        private void Reset()
        {
            _pickupType = PickupType.Key;
        }
    }
}