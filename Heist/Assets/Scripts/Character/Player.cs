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

        private bool _overTrapPickup;

        private bool _overWeaponPickup;
        [SerializeField] public PlayerControl PlayerControl;

        [field: SerializeField] public PlayerUIManager PlayerUiManager { get; }

        [field: SerializeField] public Inventory Inventory { get; }

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
            PlayerUiManager.ShowPickup(pickupType, overPickup);


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
                        Inventory.GoldAmount += ((GoldPickup) pickup).AmountOfGold;
                        Destroy(pickup.gameObject);
                        PlayerControl.PickupGold();
                    }

                    break;
                case PickupType.Key:
                    if (overPickup && pickup is KeyPickup keyPickup)
                    {
                        var destroy = !Inventory.keys[keyPickup.Key];
                        Inventory.keys[keyPickup.Key] = true;
                        if (destroy)
                        {
                            PlayerUiManager.ShowKeyPickup(keyPickup.Key);
                            PlayerUiManager.SetKeyOwned(keyPickup.Key);
                            LevelManager.LevelManagerRef.SetKeyPickedUp(keyPickup.Key);
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
                if (Inventory.Add(weapon)) Destroy(_currentlyOverPickup.gameObject);
                OverWeaponPickup = false;
                PlayerUiManager.ClearHint();
                PlayerControl.PickupWeapon();
            }
            else if (OverTrapPickup)
            {
                var hazard = (_currentlyOverPickup as HazardPickup)?.HazardGameObject;
                if (Inventory.Add(hazard)) Destroy(_currentlyOverPickup.gameObject);
                OverTrapPickup = false;
                PlayerUiManager.ClearHint();
                PlayerControl.PickupTrap();
            }
        }

        public void SetGold(int amount)
        {
            PlayerUiManager.SetGold(amount);
        }

        public QuickTimeEvent InitializeQuickTime(QuickTimeEvent quickTimeEvent)
        {
            return PlayerUiManager.InitializeQuickTime(quickTimeEvent);
        }
    }
}