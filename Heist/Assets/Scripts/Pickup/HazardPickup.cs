using Character;
using UnityEngine;

namespace Pickup
{
    public class HazardPickup : Pickup
    {
        [SerializeField] internal Hazard.Hazard HazardGameObject;

        private void Reset()
        {
            _pickupType = PickupType.Trap;
        }
    }
}