using UnityEngine;

namespace Pickup
{
    [RequireComponent(typeof(Collider))]
    public class WeaponPickup : Pickup
    {
        [SerializeField] public Weapon.Weapon WeaponGameObject;

        private void Reset()
        {
            _pickupType = PickupType.Weapon;
        }
    }
}