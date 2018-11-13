using System;
using Pickup;
using UI;
using UnityEngine;

namespace Character
{
    public class Player : Character
    {

        private Pickup.Pickup _currentlyOverPickup;

        [SerializeField] private UIManager _ui;
        [SerializeField] private Inventory _inventory;

        private bool _overWeaponPickup;

        public bool OverWeaponPickup
        {
            get { return _overWeaponPickup; }
            set
            {
                if (value != _overWeaponPickup)
                {
                    _overWeaponPickup = value;
                    _ui.ShowPickup(Pickup.Pickup.PickupType.Weapon, _overWeaponPickup);
                }
            }
        }

        private bool _overTrapPickup;

        public bool OverTrapPickup
        {
            get { return _overTrapPickup; }
            set
            {
                if (value != _overTrapPickup)
                {
                    _overTrapPickup = value;
                    _ui.ShowPickup(Pickup.Pickup.PickupType.Trap, _overTrapPickup);
                }
            }
        }

        public void OverPickup(Pickup.Pickup.PickupType pickupType, bool overPickup, Pickup.Pickup pickup)
        {
            switch (pickupType)
            {
                case Pickup.Pickup.PickupType.Weapon:
                    OverWeaponPickup = overPickup;
                    _currentlyOverPickup = overPickup ? pickup : null;
                    break;
                case Pickup.Pickup.PickupType.Trap:
                    OverWeaponPickup = overPickup;
                    _currentlyOverPickup = overPickup ? pickup : null;
                    break;
                case Pickup.Pickup.PickupType.Gold:
                    if (overPickup)
                    {
                        _inventory.GoldAmount += ((GoldPickup) pickup).AmountOfGold;
                        Destroy(pickup.gameObject);
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
                Destroy(_currentlyOverPickup.gameObject);
                _inventory.Weapon = weapon;

            }
            else if (OverTrapPickup)
            {
                var hazard = ((HazardPickup) _currentlyOverPickup).HazardGameObject;
                Destroy(_currentlyOverPickup.gameObject);
                _inventory.Hazard = hazard;
            }
        }
    }
}