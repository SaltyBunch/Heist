using System;
using Pickup;
using UI;
using UnityEngine;

namespace Character
{
    public class Player : Character
    {
        private Pickup.Pickup _currentlyOverPickup;
        [SerializeField] private Inventory _inventory;

        private bool _overTrapPickup;

        private bool _overWeaponPickup;


        public Inventory Inventory => _inventory;

        public bool OverWeaponPickup
        {
            get => _overWeaponPickup;
            set
            {
                if (value != _overWeaponPickup) _overWeaponPickup = value;
            }
        }

        public bool OverTrapPickup
        {
            get => _overTrapPickup;
            set
            {
                if (value != _overTrapPickup) _overTrapPickup = value;
            }
        }

        public void OverPickup(PickupType pickupType, bool overPickup, Pickup.Pickup pickup)
        {
            switch (pickupType)
            {
                case PickupType.Weapon:
                    OverWeaponPickup = overPickup;
                    _currentlyOverPickup = overPickup ? pickup : null;
                    break;
                case PickupType.Trap:
                    OverWeaponPickup = overPickup;
                    _currentlyOverPickup = overPickup ? pickup : null;
                    break;
                case PickupType.Gold:
                    if (overPickup)
                    {
                        _inventory.GoldAmount += ((GoldPickup) pickup).AmountOfGold;
                        Destroy(pickup.gameObject);
                    }
                    break;
                case PickupType.Key:
                    if (overPickup && pickup is KeyPickup keyPickup)
                    {
                        var destroy = !_inventory.keys[keyPickup.Key];
                        _inventory.keys[keyPickup.Key] = true;
                        if (destroy) Destroy(pickup.gameObject);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pickupType), pickupType, null);
            }
        }

        public void PickupPickup()
        {
            if (OverWeaponPickup)
            {
                var weapon = ((WeaponPickup) _currentlyOverPickup).WeaponGameObject;
                if (_inventory.Add(weapon)) Destroy(_currentlyOverPickup.gameObject);
            }
            else if (OverTrapPickup)
            {
                var hazard = ((HazardPickup) _currentlyOverPickup).HazardGameObject;
                if (_inventory.Add(hazard)) Destroy(_currentlyOverPickup.gameObject);
            }
        }
    }
}