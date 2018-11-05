using Character;
using UnityEngine;

namespace Pickup
{
    [RequireComponent(typeof(Collider))]
    public class GoldPickup : Pickup
    {
        private void Reset()
        {
            _pickupType = PickupType.Gold;
        }
    }
}