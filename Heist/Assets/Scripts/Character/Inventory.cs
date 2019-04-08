using System.Collections.Generic;
using System.Linq;
using Game;
using Hazard;
using UnityEngine;
using Weapon;

namespace Character
{
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    public delegate void GoldAmountChangedHandler(object sender, GoldAmountChangedEvent e);

    public class GoldAmountChangedEvent
    {
        public int Amount;
    }

    public class SelectionChangedEventArgs
    {
        public int Count;
        public Item Item;
        public Item.Type Type;
    }


    public class Inventory : MonoBehaviour
    {
        private int _count;
        [SerializeField] private List<Hazard.Hazard> _hazard = new List<Hazard.Hazard>();

        [SerializeField] public Dictionary<KeyType, bool> keys = new Dictionary<KeyType, bool>
        {
            {KeyType.BlueKey, false}, {KeyType.YellowKey, false}, {KeyType.RedKey, false}
        };

        [SerializeField] private PlayerControl _player;
        private int _selectedHazard;

        private int _selectedIndex;
        private int _selectedWeapon;
        private Item.Type _type;

        [SerializeField] private List<Weapon.Weapon> _weapon = new List<Weapon.Weapon>();
        private int _goldAmount;

        public int GoldAmount
        {
            get => _goldAmount;
            set
            {
                var dif = value - _goldAmount;
                _goldAmount = value;
                _player.BaseCharacter.SetGold(_goldAmount);

                GoldAmountChanged?.Invoke(this, new GoldAmountChangedEvent
                {
                    Amount = dif
                });
            }
        }

        public Item SelectedItem { get; private set; }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                var val = 0;
                if (_weapon.Count + _hazard.Count != 0)
                    val = value % (_weapon.Count + _hazard.Count); // normalize to range of values
                if (val < 0) val += _weapon.Count + _hazard.Count;
                if (val - _selectedIndex != 0) //selection has changed
                {
                    //get direction of selection changed
                    var dir = (int) Mathf.Sign(value - _selectedIndex);
                    if (val >= _weapon.Count && val < _hazard.Count + _weapon.Count &&
                        _selectedIndex >= _weapon.Count &&
                        _selectedIndex < _hazard.Count + _weapon.Count
                    ) //a hazard is selected and the prev selected is a hazard
                    {
                        var prevVal = false;
                        //iterate in the direction of dir until selection at index != selection at _selected index or until index is <> _hazard.Count
                        for (var index = (_selectedIndex - _weapon.Count) % _hazard.Count;
                            index < _hazard.Count && index >= 0;
                            index += dir)
                            if (_hazard[index].GetType() !=
                                _hazard[(_selectedIndex - _weapon.Count) % _hazard.Count].GetType())
                            {
                                val = index + _weapon.Count;
                                prevVal = true;
                                break;
                            }

                        if (!prevVal) //exited for loop without finding a new hazard to select
                            switch (dir)
                            {
                                case 1:
                                    val = 0;
                                    break;
                                case -1:
                                    val = _weapon.Count - 1;
                                    break;
                            }
                    }
                    else if (val >= _weapon.Count && val < _hazard.Count + _weapon.Count)
                        //new selection is a hazard but old one isnt
                    {
                        switch (dir)
                        {
                            case 1:
                                val = _weapon.Count;
                                break;
                            case -1:
                                val = _weapon.Count + _hazard.Count - 1;
                                break;
                        }
                    }
                }

                _selectedIndex = val;
                if (_weapon.Count + _hazard.Count != 0)
                    SelectedItem = _selectedIndex >= _weapon.Count
                        ? _hazard[_selectedIndex - _weapon.Count]
                        : (Item) _weapon[_selectedIndex];
                else
                    SelectedItem = null;
                _type = SelectedItem is Weapon.Weapon ? Item.Type.Weapon : Item.Type.Hazard;
                _count = _type == Item.Type.Weapon
                    ? ((Weapon.Weapon) SelectedItem).Ammo
                    : _type == Item.Type.Hazard
                        ? _hazard.Count(h => h.GetType() == SelectedItem.GetType())
                        : 0;
                SelectionChanged?.Invoke(this,
                    new SelectionChangedEventArgs
                    {
                        Item = SelectedItem,
                        Type = _type,
                        Count = _count
                    });
            }
        }


        public event SelectionChangedEventHandler SelectionChanged;
        public event GoldAmountChangedHandler GoldAmountChanged;

        private void Awake()
        {
            _player = GetComponent<PlayerControl>();
        }

        private void Reset()
        {
            _player = GetComponent<PlayerControl>();
        }

        public bool Add(Item item)
        {
            if (item != null)
            {
                item = Instantiate(item, transform);
                item.gameObject.SetActive(false);
            }

            if (item is Weapon.Weapon weapon)
            {
                GameManager.SetLayerOnAll(item.gameObject, 11);
                if (_weapon.Contains(weapon))
                {
                    var temp = _weapon.Find(x => x.GetType() == weapon.GetType());
                    temp.Ammo += weapon.Ammo;
                    Destroy(weapon.gameObject);
                }
                else
                {
                    _weapon.Add(weapon);
                }

                SelectedIndex = _weapon.Count - 1;
                return true;
            }

            if (item is Hazard.Hazard hazard)
            {
                var count = _hazard.Count(h => h.GetType() == hazard.GetType());
                if (count < 2)
                {
                    var index = 0;
                    for (var i = 0; i < _hazard.Count; i++)
                    {
                        if (_hazard[i].GetType() == hazard.GetType())
                        {
                            //hazard at i is the same as hazard
                            //insert at i and break
                            _hazard.Insert(i, hazard);
                            break;
                        }

                        index = i;
                    }

                    if (_hazard.Count == 0 || index == _hazard.Count - 1) _hazard.Add(hazard);
                }

                SelectedIndex = _hazard.FindIndex(h => h == hazard) + _weapon.Count;
                return count < 2;
            }

            return false;
        }

        public bool Use()
        {
            RaycastHit hit;
            switch (SelectedItem)
            {
                case ElectricField electricField:
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 2,
                        LevelManager.LevelManagerRef.EnvironmentLayer)
                        ? electricField.Place(hit.point - transform.forward / 2)
                        : electricField.Place(transform.position + transform.forward * 2))
                    {
                        electricField.transform.parent = null;
                        Remove(SelectedItem);
                        electricField.PlacedByPlayer = true;
                        electricField.gameObject.SetActive(true);
                        return true;
                    }

                    return false;
                case LethalLaser lethalLaser:
                    if (Physics.Raycast(transform.position, transform.forward, out hit, 2,
                        LevelManager.LevelManagerRef.EnvironmentLayer)
                        ? lethalLaser.Place(hit.point - transform.forward / 2)
                        : lethalLaser.Place(transform.position + transform.forward * 2))
                    {
                        lethalLaser.transform.parent = null;
                        Remove(SelectedItem);
                        lethalLaser.PlacedByPlayer = true;
                        lethalLaser.gameObject.SetActive(true);
                        return true;
                    }

                    return false;
                case Baton baton:
                    baton.Use();
                    LevelManager.LevelManagerRef.Notify(transform.position, NotifyType.Attack);
                    _count = baton.Ammo;
                    if (_count == 0)
                    {
                        Remove(SelectedItem);
                        return true;
                    }

                    break;
                case StunGun stunGun:
                    stunGun.Attack();
                    LevelManager.LevelManagerRef.Notify(transform.position, NotifyType.Attack);
                    _count = stunGun.Ammo;
                    if (_count == 0)
                    {
                        Remove(SelectedItem);
                        return true;
                    }

                    break;
                case Shotgun shotgun:
                    shotgun.Attack();
                    LevelManager.LevelManagerRef.Notify(transform.position, NotifyType.Attack);
                    _count = shotgun.Ammo;
                    if (_count == 0)
                    {
                        Remove(SelectedItem);
                        return true;
                    }

                    break;
            }

            SelectionChanged?.Invoke(this,
                new SelectionChangedEventArgs
                {
                    Item = SelectedItem,
                    Type = _type,
                    Count = _count
                });
            return true;
        }

        private void Remove(Item selectedItem)
        {
            //remove item from list
            if (selectedItem is Weapon.Weapon weapon)
            {
                for (int i = 0; i < _weapon.Count; i++)
                {
                    if (_weapon[i] == weapon)
                    {
                        var weap = _weapon[i];
                        _weapon.RemoveAt(i);
                        Destroy(weap.gameObject);
                        break;
                    }
                }

                SelectedIndex = 0;
            }
            else if (selectedItem is Hazard.Hazard hazard)
            {
                int index = 0;
                for (int j = 0; j < _hazard.Count; j++)
                {
                    if (_hazard[j] == hazard)
                    {
                        index = j;
                        _hazard.RemoveAt(j);
                        if (_hazard.Count(h => h == hazard) == 0)
                        {
                            index = 0;
                        }

                        break;
                    }
                }

                SelectedIndex = index;
            }
        }
    }
}