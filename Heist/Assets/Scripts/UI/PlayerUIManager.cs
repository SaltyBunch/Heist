using System;
using System.Collections;
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
        [SerializeField] private TextMeshProUGUI _playerInfo;
        [SerializeField] private TextMeshProUGUI _serviceAnnouncement;
        [SerializeField] private TextMeshProUGUI _controlHint;
        public GameObject Siren => _playerStatsManager.Siren;


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
                    uiLocation.anchorMin = Vector2.zero;

                    uiLocation.pivot = Vector2.one;

                    _playerStatsManager.SwitchSides(Side.Right, playerNumber);
                }
                else //left
                {
                    //top left
                    uiLocation.anchorMax = Vector2.one;
                    uiLocation.anchorMin = Vector2.zero;

                    uiLocation.pivot = Vector2.up;

                    _playerStatsManager.SwitchSides(Side.Left, playerNumber);
                }
            }
            else //bottom
            {
                if (Math.Abs(rect.x) > 0.01f) //put right
                {
                    //bottom right
                    uiLocation.anchorMax = Vector2.one;
                    uiLocation.anchorMin = Vector2.zero;

                    uiLocation.pivot = Vector2.right;

                    _playerStatsManager.SwitchSides(Side.Right, playerNumber);
                }
                else
                {
                    //bottom left
                    uiLocation.anchorMax = Vector2.one;
                    uiLocation.anchorMin = Vector2.zero;

                    uiLocation.pivot = Vector2.zero;

                    _playerStatsManager.SwitchSides(Side.Left, playerNumber);
                }
            }
        }

        public void SetHealth(int amount) => _playerStatsManager.SetHealth(amount);

        public void NotifyMessage(object sender, NotifyMessageArgs notifyMessageArgs) =>
            _playerStatsManager.NotifyAll(notifyMessageArgs.Message);

        public void SetGold(int amount) => _playerStatsManager.SetGold(amount);

        public void SetCharacter(Game.Characters character) => _playerStatsManager.SetCharacter(character);

        public void SetItem(Item item) => _playerStatsManager.SetItem(item);

        public void UpdateAmmo(int amount) => _playerStatsManager.SetAmmo(amount);

        public void SetKeyPickedUp(KeyType keyType) => _playerStatsManager.SetKeyPickedUp(keyType);
        public void SetKeyOwned(KeyType keyType) => _playerStatsManager.SetKeyOwned(keyType);

        public TextMeshProUGUI VaultTimer => _playerStatsManager.VaultTimer;

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

        public void GetGold(int amount)
        {
            if (amount > 0)
            {
                _serviceAnnouncement.text = "<#38db32>+$" + amount;
            }
            else if (amount < 0)
            {
                _serviceAnnouncement.text = "<#f92a2a>-$" + Mathf.Abs(amount);
            }

            StartCoroutine(ClearHintIn(1, _serviceAnnouncement));
        }

        public void ClearHint(TextMeshProUGUI textBox)
        {
            textBox.text = "";
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
                    ClearHint(_playerHint);
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
                    ClearHint(_playerInfo);
                    break;
            }

            StartCoroutine(ClearHintIn(5, _playerInfo));
        }

        private IEnumerator ClearHintIn(float time, TextMeshProUGUI textBox)
        {
            yield return new WaitForSeconds(time);
            ClearHint(textBox);
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
                    ClearHint(_playerInfo);
                    break;
            }

            StartCoroutine(ClearHintIn(2, _playerInfo));
        }

        public void SetControlHint(Item.Type type)
        {
            switch (type)
            {
                case Item.Type.None:
                    _controlHint.text = "";
                    break;
                case Item.Type.Weapon:
                    _controlHint.text = "Press RT to use weapon";
                    break;
                case Item.Type.Hazard:
                    _controlHint.text = "Press RT to use trap";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            StartCoroutine(ClearHintIn(2, _controlHint));
        }

        public void NeedsBothKeys()
        {
            _playerInfo.text = TextHelper.RequireBothKeys;
            StartCoroutine(ClearHintIn(2, _playerInfo));
        }

        public void ClearPlayerInfo() => ClearHint(_playerInfo);
    }
}