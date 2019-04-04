using System;
using System.Collections;
using Game;
using Pickup;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerUIManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerHint;
        [SerializeField] private TextMeshProUGUI _playerInfo;
        [SerializeField] private PlayerStatsManager _playerStatsManager;

        [SerializeField] private Transform _quickTimePosition;
        public GameObject Siren => _playerStatsManager.Siren;

        public TextMeshProUGUI VaultTimer => _playerStatsManager.VaultTimer;


        public void SetPosition(Rect rect, int playerNumber)
        {
            var uiLocation = _playerStatsManager.GetComponent<RectTransform>();

            var pos = new Rect
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

        public void SetHealth(int amount)
        {
            _playerStatsManager.SetHealth(amount);
        }

        public void NotifyMessage(object sender, NotifyMessageArgs notifyMessageArgs)
        {
            _playerStatsManager.NotifyAll(notifyMessageArgs.Message);
        }

        public void SetGold(int amount)
        {
            _playerStatsManager.SetGold(amount);
        }

        public void SetCharacter(Characters character)
        {
            _playerStatsManager.SetCharacter(character);
        }

        public void SetItem(Item item)
        {
            _playerStatsManager.SetItem(item);
        }

        public void UpdateAmmo(int amount)
        {
            _playerStatsManager.SetAmmo(amount);
        }

        public void SetKeyPickedUp(KeyType keyType)
        {
            _playerStatsManager.SetKeyPickedUp(keyType);
        }

        public void SetKeyOwned(KeyType keyType)
        {
            _playerStatsManager.SetKeyOwned(keyType);
        }

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

        public void SetOpen(string type, bool open)
        {
            switch (type)
            {
                case "Door":
                    _playerHint.text = open ? TextHelper.CloseDoor : TextHelper.OpenDoor;
                    break;
                case "HazardDisabler":
                    _playerHint.text = TextHelper.DisableHazard;
                    break;
                case "Vault":
                    _playerHint.text = TextHelper.OpenVault;
                    break;
                case "MiniVault":
                    _playerHint.text = TextHelper.OpenMiniVault;
                    break;
                case "GoldPile":
                    _playerHint.text = TextHelper.TakeGold;
                    break;
                default:
                    ClearHint();
                    break;
            }
        }

        public void ShowKeyPickup(KeyType keyPickupKey)
        {
            switch (keyPickupKey)
            {
                case KeyType.BlueKey:
                    _playerInfo.text = TextHelper.BlueKeyPickUp;
                    break;
                case KeyType.RedKey:
                    _playerInfo.text = TextHelper.RedKeyPickUp;
                    break;
                case KeyType.YellowKey:
                    _playerInfo.text = TextHelper.YellowKeyPickUp;
                    break;
                default:
                    ClearHint();
                    break;
            }

            StartCoroutine(ClearHintIn(5));
        }

        private IEnumerator ClearHintIn(float time)
        {
            yield return new WaitForSeconds(time);
            ClearInfo();
        }

        private void ClearInfo()
        {
            _playerInfo.text = "";
        }

        public void NeedKey(KeyType key)
        {
            switch (key)
            {
                case KeyType.BlueKey:
                    _playerInfo.text = TextHelper.RequireBlueKey;
                    break;
                case KeyType.RedKey:
                    _playerInfo.text = TextHelper.RequireRedKey;
                    break;
                case KeyType.YellowKey:
                    _playerInfo.text = TextHelper.RequireYellowKey;
                    break;
                default:
                    ClearHint();
                    break;
            }

            StartCoroutine(ClearHintIn(5));
        }

        public void NeedsBothKeys()
        {
            _playerInfo.text = TextHelper.RequireBothKeys;
            StartCoroutine(ClearHintIn(5));
        }
    }
}