using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _playerHint;

        public void ShowPickup(Pickup.Pickup.PickupType pickupType, bool overPickup)
        {
            if (!overPickup)
            {
                _playerHint.text = "";
                return;
            }
            switch (pickupType)
            {
                case Pickup.Pickup.PickupType.Weapon:
                    _playerHint.text = TextHelper.PickupWeapon;
                    break;
                case Pickup.Pickup.PickupType.Trap:
                    _playerHint.text = TextHelper.PickupTrap;
                    break;
                case Pickup.Pickup.PickupType.Gold:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pickupType), pickupType, null);
            }
        }
    }
}