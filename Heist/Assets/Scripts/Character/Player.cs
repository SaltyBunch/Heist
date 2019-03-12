using System;
using Game;
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

        [SerializeField] private PlayerUIManager _playerUiManager;

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
            _playerUiManager.ShowPickup(pickupType, overPickup);


            switch (pickupType)
            {
                case PickupType.Weapon:
                    OverWeaponPickup = overPickup;
                    _currentlyOverPickup = overPickup ? pickup : null;
                    break;
                case PickupType.Trap:
                    OverTrapPickup = overPickup;
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
                        if (destroy)
                        {
                            LevelManager.LevelManagerRef.NotifyPlayers("A player has picked up the " + keyPickup.Key);
                            Destroy(pickup.gameObject);
                        }
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
                var weapon = (_currentlyOverPickup as WeaponPickup)?.WeaponGameObject;
                if (_inventory.Add(weapon)) Destroy(_currentlyOverPickup.gameObject);
                OverWeaponPickup = false;
                _playerUiManager.ClearHint();
            }
            else if (OverTrapPickup)
            {
                var hazard = (_currentlyOverPickup as HazardPickup)?.HazardGameObject;
                if (_inventory.Add(hazard)) Destroy(_currentlyOverPickup.gameObject);
                OverTrapPickup = false;
                _playerUiManager.ClearHint();
            }
        }

        public void SetGold(int amount) => _playerUiManager.SetGold(amount);

        public QuickTimeEvent InitializeQuickTime(QuickTimeEvent quickTimeEvent) =>
            _playerUiManager.InitializeQuickTime(quickTimeEvent);
    }
}