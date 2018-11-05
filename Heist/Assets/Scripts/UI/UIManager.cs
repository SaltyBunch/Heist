using System;
using UnityEngine;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public void ShowPickup(Pickup.Pickup.PickupType pickupType, bool overTrapPickup)
        {
            switch (pickupType)
            {
                case Pickup.Pickup.PickupType.Weapon:
                    break;
                case Pickup.Pickup.PickupType.Trap:
                    break;
                case Pickup.Pickup.PickupType.Gold:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pickupType), pickupType, null);
            }
        }
    }
}