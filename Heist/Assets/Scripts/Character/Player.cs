using System;
using Pickup;
using UI;
using UnityEngine;

namespace Character
{
    public class Player : Character
    {
        private Pickup.Pickup _currentlyOverPickup;

        [SerializeField] private UIManager _ui => UIManager.UiManagerRef;
        [SerializeField] private Inventory _inventory;

        public Inventory Inventory => _inventory;

        private bool _overWeaponPickup;

        public bool OverWeaponPickup
        {
            get { return _overWeaponPickup; }
            set
            {
                if (value != _overWeaponPickup)
                {
                    _overWeaponPickup = value;
                    _ui.ShowPickup(Pickup.PickupType.Weapon, _overWeaponPickup);
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
                    _ui.ShowPickup(Pickup.PickupType.Trap, _overTrapPickup);
                }
            }
        }

        public void OverPickup(Pickup.PickupType pickupType, bool overPickup, Pickup.Pickup pickup)
        {
            switch (pickupType)
            {
                case Pickup.PickupType.Weapon:
                    OverWeaponPickup = overPickup;
                    _currentlyOverPickup = overPickup ? pickup : null;
                    break;
                case Pickup.PickupType.Trap:
                    OverWeaponPickup = overPickup;
                    _currentlyOverPickup = overPickup ? pickup : null;
                    break;
                case Pickup.PickupType.Gold:
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
                if (_inventory.Add(weapon))
                {
                    Destroy(_currentlyOverPickup.gameObject);
                }
            }
            else if (OverTrapPickup)
            {
                var hazard = ((HazardPickup) _currentlyOverPickup).HazardGameObject;
                if (_inventory.Add(hazard))
                {
                    Destroy(_currentlyOverPickup.gameObject);
                }
            }
        }
    }
}