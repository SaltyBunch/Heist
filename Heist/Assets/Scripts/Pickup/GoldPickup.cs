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

        public void SetDisableFor(int playerNumber)
        {
            _ignorePlayer = playerNumber;
        }

        public void SetEnableAll()
        {
            _ignorePlayer = -1;
        }
    }
}