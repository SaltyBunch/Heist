using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] List<Sprite> Portraits;    //king[0], jailbird[1], shadow[2], raccon[3]
        [SerializeField] List<Image> faces;
        [SerializeField] List<PLayerHP> AllHP;
        [SerializeField] List<Text> AllAmmo;
        

        [SerializeField] List<GameObject> PlayerUI;
        [SerializeField] List<Text> GoldUI;
        [SerializeField] List<Sprite> ItemUI;   //Gun[0], stick[1], lethal[2], field[3]
        [SerializeField] List<Image> curItemUi;
        float[] showWeapon = { 5f, 5f, 5f, 5f };
        bool[] showingWeapon = { true, true, true, true };
        private Character.Inventory inv;

        [SerializeField] private TextMeshProUGUI _playerHint;

        private void Update()
        {
            for (int i = 0; i < showWeapon.Length; i++)
            {
                showWeapon[i] -= Time.deltaTime;
            }

            for (int i = 0; i < showWeapon.Length; i++)
            {
                if (showWeapon[i] <= 0 && showingWeapon[i])
                {
                    showingWeapon[i] = false;
                    StartCoroutine(FadeWeapon(i));
                }
            }
        }


        public void CreateUI(int i)
        {
            PlayerUI[i].SetActive(true);
            showingWeapon = new bool[i];
            showWeapon = new float[i];
            for (int j = 0; j < i; j++)
            {
                showWeapon[i] = 5f;
            }
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
        public void UpdateHealth(int hp, int player)
        {
            for (int i = 0; i < AllHP[player].playerhp.Count; i++)
            {
                AllHP[player].playerhp[i].SetActive(i < hp);
            }
        }

        public void SetFace(int chara, int player)
        {
            faces[player].sprite = Portraits[chara];
        }

        public void UpdateGold(int gold, int player)
        {
            GoldUI[player].text = gold.ToString();
        }

        public void UpdateAmmo(int amount, int player)
        {
            AllAmmo[player].text = amount.ToString();
        }

        public void UpdateWeapon(Game.Item item, int player)
        {
            if (item is Weapon.Weapon)
            {
                if (item is Weapon.StunGun)
                {
                    curItemUi[player].sprite = ItemUI[0];
                }
                else if (item is Weapon.Baton)
                {
                    curItemUi[player].sprite = ItemUI[1];
                }
            }
            else if (item is Hazard.Hazard)
            {
                if (item is Hazard.ElectricField)
                {
                    curItemUi[player].sprite = ItemUI[2];
                }
                else if (item is Hazard.LethalLaser)
                {
                    curItemUi[player].sprite = ItemUI[3];
                }
            }

            showingWeapon[player] = true;
            showWeapon[player] = 5f;
        }
        public IEnumerator FadeWeapon(int player)
        {
            yield return new WaitForEndOfFrame();
            var temp = curItemUi[player].GetComponent<RectTransform>();

            for (int i = 0; i < 30; i++)
            {
                switch (player)
                {
                    case 0:
                        temp.position = new Vector3(temp.position.x, temp.position.y + 300 * Time.deltaTime, 0);
                        break;
                    case 1:
                        temp.position = new Vector3(temp.position.x, temp.position.y + 300 * Time.deltaTime, 0);
                        break;
                    case 2:
                        temp.position = new Vector3(temp.position.x, temp.position.y - 300 * Time.deltaTime, 0);
                        break;
                    case 3:
                        temp.position = new Vector3(temp.position.x, temp.position.y - 300 * Time.deltaTime, 0);
                        break;
                }
                yield return null;
            }
        }
    }



    [System.Serializable]
    public class PLayerHP
    {
        public List<GameObject> playerhp;
    }
}