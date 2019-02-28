using System;
using Game;
using Hazard;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapon;

namespace UI
{
    public enum Side
    {
        Right,
        Left
    }

    public class PlayerStatsManager : MonoBehaviour
    {
        [SerializeField] private Sprite[] _playerIcons;

        [SerializeField] private TextMeshProUGUI _gold;
        [SerializeField] private Image[] _health;
        [SerializeField] private Image _playerPortrait;
        [SerializeField] private Image _itemPortrait;

        [SerializeField] private Image _playerBorder;
        [SerializeField] private Image _itemBorder;

        [SerializeField] private TextMeshProUGUI _ammoText;

        [SerializeField] private Sprite[] _borders;
        [SerializeField] private Sprite[] _itemBorders;

        [SerializeField] private Sprite[] _characters;
        [SerializeField] private Sprite[] _items;

        public void SwitchSides(Side side, int playerNum)
        {
            var playerRect = _playerPortrait.gameObject.GetComponent<RectTransform>();
            var itemRect = _itemPortrait.gameObject.GetComponent<RectTransform>();

            switch (side)
            {
                case Side.Right:
                    playerRect.pivot = Vector2.right * 1.5f + Vector2.up * 0.5f;
                    itemRect.pivot = Vector2.right * -0.5f + Vector2.up * 0.5f;

                    playerRect.anchorMax = Vector2.right + Vector2.up * 0.5f;
                    playerRect.anchorMin = Vector2.right + Vector2.up * 0.5f;

                    itemRect.anchorMax = Vector2.up * 0.5f;
                    itemRect.anchorMin = Vector2.up * 0.5f;


                    _playerBorder.gameObject.transform.localScale = new Vector3()
                    {
                        x = -1, y = 1, z = 1
                    };
                    break;
                case Side.Left:
                    playerRect.pivot = Vector2.right * -0.5f + Vector2.up * 0.5f;
                    itemRect.pivot = Vector2.right * 1.5f + Vector2.up * 0.5f;

                    itemRect.anchorMax = Vector2.right + Vector2.up * 0.5f;
                    itemRect.anchorMin = Vector2.right + Vector2.up * 0.5f;

                    playerRect.anchorMax = Vector2.up * 0.5f;
                    playerRect.anchorMin = Vector2.up * 0.5f;

                    _playerBorder.gameObject.transform.localScale = new Vector3()
                    {
                        x = 1, y = 1, z = 1
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(side), side, null);
            }

            _playerBorder.sprite = _borders[playerNum];
            _itemBorder.sprite = _itemBorders[playerNum];
        }

        public void SetHealth(int num)
        {
            for (int i = 0; i < _health.Length; i++)
                _health[i].gameObject.SetActive(i < num);
        }

        public void SetGold(int amount)
        {
            _gold.SetText("$" + amount);
        }

        public void SetCharacter(Game.Characters character)
        {
            _playerPortrait.sprite = _characters[(int) character];
        }

        public void SetItem(Item item)
        {
            if (item == null)
            {
                _itemPortrait.sprite = _items[4];
            }

            switch (item)
            {
                case ElectricField electricField:
                    _itemPortrait.sprite = _items[2];
                    break;
                case LethalLaser lethalLaser:
                    _itemPortrait.sprite = _items[3];
                    break;
                case Baton baton:
                    _itemPortrait.sprite = _items[0];
                    break;
                case StunGun stunGun:
                    _itemPortrait.sprite = _items[1];
                    break;
            }
        }

        public void SetAmmo(int amount)
        {
            _ammoText.SetText(amount != 0 ? "" + amount : "");
        }
    }
}