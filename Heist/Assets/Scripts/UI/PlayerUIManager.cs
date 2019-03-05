using System;
using Game;
using Pickup;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private PlayerStatsManager _playerStatsManager;

        [SerializeField] private Transform _quickTimePosition;

        [SerializeField] private TextMeshProUGUI _playerHint;

        public void SetPosition(Rect rect, int playerNumber)
        {
            var uiLocation = _playerStatsManager.GetComponent<RectTransform>();

            var pos = new Rect()
            {
                x = 0, y = 0
            };

            //todo
            //todo
            //todo
            if (Math.Abs(rect.y) > 0.01f) //put top
            {
                if (Math.Abs(rect.x) > 0.01f) //put right
                {
                    //top right
                    uiLocation.anchorMax = Vector2.one;
                    uiLocation.anchorMin = Vector2.one;

                    uiLocation.pivot = Vector2.one;

                    _playerStatsManager.SwitchSides(Side.Right, playerNumber);
                }
                else //left
                {
                    //top left
                    uiLocation.anchorMax = Vector2.up;
                    uiLocation.anchorMin = Vector2.up;

                    uiLocation.pivot = Vector2.up;

                    _playerStatsManager.SwitchSides(Side.Left, playerNumber);
                }
            }
            else //bottom
            {
                if (Math.Abs(rect.x) > 0.01f) //put right
                {
                    //bottom right
                    uiLocation.anchorMax = Vector2.right;
                    uiLocation.anchorMin = Vector2.right;

                    uiLocation.pivot = Vector2.right;

                    _playerStatsManager.SwitchSides(Side.Right, playerNumber);
                }
                else
                {
                    //bottom left
                    uiLocation.anchorMax = Vector2.zero;
                    uiLocation.anchorMin = Vector2.zero;

                    uiLocation.pivot = Vector2.zero;

                    _playerStatsManager.SwitchSides(Side.Left, playerNumber);
                }
            }
        }

        public void SetHealth(int amount) => _playerStatsManager.SetHealth(amount);

        public void SetGold(int amount) => _playerStatsManager.SetGold(amount);

        public void SetCharacter(Game.Characters character) => _playerStatsManager.SetCharacter(character);

        public void SetItem(Item item) => _playerStatsManager.SetItem(item);

        public void UpdateAmmo(int amount) => _playerStatsManager.SetAmmo(amount);

        public void ShowPickup(PickupType pickupType, bool overPickup)
        {
            if (!overPickup)
            {
                _playerHint.text = "";
                return;
            }

            switch (pickupType)
            {
                case PickupType.Weapon:
                    _playerHint.text = TextHelper.PickupWeapon;
                    break;
                case PickupType.Trap:
                    _playerHint.text = TextHelper.PickupTrap;
                    break;
                case PickupType.Gold:
                    break;
                case PickupType.Key:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(pickupType), pickupType, null);
            }
        }

        public void ClearHint()
        {
            _playerHint.text = "";
        }

        public QuickTimeEvent InitializeQuickTime(QuickTimeEvent quickTimeEvent)
        {
            return Instantiate(quickTimeEvent, _quickTimePosition);
        }
    }
}