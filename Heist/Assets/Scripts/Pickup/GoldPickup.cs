using UnityEngine;

namespace Pickup
{
    [RequireComponent(typeof(Collider))]
    public class GoldPickup : Pickup
    {
        public int AmountOfGold;

        private void Reset()
        {
            _pickupType = PickupType.Gold;
        }
    }
}