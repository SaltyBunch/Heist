using Character;
using UnityEngine;

namespace Pickup
{
    [RequireComponent(typeof(Collider))]
    public class WeaponPickup : Pickup
    {
        [SerializeField] private Weapon.Weapon _weaponGameObject;
        
        private void Reset()
        {
            _pickupType = PickupType.Weapon;
        }
    }
}