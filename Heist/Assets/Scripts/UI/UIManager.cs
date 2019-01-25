using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI _playerHint;

        [SerializeField] List<GameObject> PlayerUI;
        [SerializeField] List<GameObject> HealthUI;
        [SerializeField] Text GoldUI;
        [SerializeField] List<GameObject> ItemUI;
        [SerializeField] private GameObject curItemUi;
        private int curWeap;
        private Character.Inventory inv;

        public void CreateUI(int i)
        {
            PlayerUI[i].SetActive(true);
        }

        public void ShowPickup(Pickup.PickupType pickupType, bool overPickup)
        {
            if (!overPickup)
            {
                _playerHint.text = "";
                return;
            }
            switch (pickupType)
            {
                case Pickup.PickupType.Weapon:
                    _playerHint.text = TextHelper.PickupWeapon;
                    break;
                case Pickup.PickupType.Trap:
                    _playerHint.text = TextHelper.PickupTrap;
                    break;
                case Pickup.PickupType.Gold:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pickupType), pickupType, null);
            }
        }
        public void UpdateHealth(int hp)
        {
            for (int i = 0; i < HealthUI.Count; i++)
            {
                HealthUI[i].SetActive(i < hp);
            }
        }

        public void UpdateGold(int goldA)
        {
            GoldUI.text = goldA.ToString();
        }

        public void UpdateWeapon(Game.Item item)
        {
            if (item is Weapon.Weapon)
            {
                if (item is Weapon.StunGun)
                {
                    curItemUi.SetActive(false);
                    curItemUi = ItemUI[0];
                    curItemUi.SetActive(true);
                }
                else if (item is Weapon.Baton)
                {
                    curItemUi.SetActive(false);
                    curItemUi = ItemUI[1];
                    curItemUi.SetActive(true);
                }
            }
            else if (item is Hazard.Hazard)
            {
                if (item is Hazard.ElectricField)
                {
                    curItemUi.SetActive(false);
                    curItemUi = ItemUI[2];
                    curItemUi.SetActive(true);
                }
                else if (item is Hazard.LethalLaser)
                {
                    curItemUi.SetActive(false);
                    curItemUi = ItemUI[3];
                    curItemUi.SetActive(true);
                }
            }
        }
    }
}