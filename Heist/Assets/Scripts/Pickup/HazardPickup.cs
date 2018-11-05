using Character;
using UnityEngine;

namespace Pickup
{
    public class HazardPickup : Pickup
    {
        [SerializeField] private Hazard.Hazard _hazardGameObject;

        private void Reset()
        {
            _pickupType = PickupType.Trap;
        }
    }
}