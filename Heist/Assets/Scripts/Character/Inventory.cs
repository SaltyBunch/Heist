using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace Character
{
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    public class SelectionChangedEventArgs
    {
        public int Count;
        public Item Item;
        public Item.Type Type;
    }


    public class Inventory : MonoBehaviour
    {
        [SerializeField] private PlayerControl _player;
        private int _selectedHazard;

        private int _selectedIndex;
        private int _selectedWeapon;

        public int GoldAmount;

        [SerializeField] private List<Weapon.Weapon> _weapon = new List<Weapon.Weapon>();
        [SerializeField] private List<Hazard.Hazard> _hazard = new List<Hazard.Hazard>();
        private Item _selectedItem;

        public Item SelectedItem
        {
            get { return _selectedItem; }
        }

        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                var val = value % (_weapon.Count + _hazard.Count); // normalize to range of values


                if (val - _selectedIndex != 0) //selection has changed
                {
                    //get direction of selection changed
                    int dir = (int) Mathf.Sign(value - _selectedIndex);

                    Debug.Log(dir);

                    if (val >= _weapon.Count && val < _hazard.Count + _weapon.Count) //a hazard is selected
                    {
                        //iterate in the direction of dir until selection at index != selection at _selected index or until index is <> _hazard.Count
                        for (int index = _selectedIndex - _weapon.Count;
                            index < _hazard.Count && index >= 0;
                            index += dir)
                        {
                            if (_hazard[index] != _hazard[_selectedIndex])
                            {
                                val = index + _hazard.Count;
                                break;
                            }
                        }
                    }
                }

                //
                _selectedIndex = val;
                _selectedItem = _selectedIndex >= _weapon.Count
                    ? _hazard[_selectedIndex % _weapon.Count]
                    : (Item) _weapon[_selectedIndex];

                var type = _selectedItem is Weapon.Weapon ? Item.Type.Weapon : Item.Type.Hazard;


                SelectionChanged?.Invoke(this,
                    new SelectionChangedEventArgs
                    {
                        Item = _selectedItem,
                        Type = type,
                        Count = type == Item.Type.Weapon
                            ? ((Weapon.Weapon) _selectedItem).Ammo
                            : _hazard.Count(h => h == _selectedItem) //todo better
                    });
            }
        }


        public event SelectionChangedEventHandler SelectionChanged;

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
            if (item is Weapon.Weapon weapon)
            {
                if (_weapon.Contains(weapon))
                {
                    var temp = _weapon.Find(x => x == weapon);
                    _weapon.Remove(temp);
                    Destroy(temp.gameObject);
                    _weapon.Add(weapon);
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
                var count = _hazard.Count(h => h == hazard);
                if (count < 2)
                {
                    int index = 0;
                    for (int i = 0; i < _hazard.Count; i++)
                    {
                        if (_hazard[i] == hazard)
                        {
                            //hazard at i is the same as hazard
                            //insert at i and break
                            _hazard.Insert(i, hazard);
                            break;
                        }

                        index = i;
                    }

                    if (_hazard.Count == 0 || index == _hazard.Count - 1)
                    {
                        //reached the end without inserting the hazard
                        _hazard.Add(hazard);
                    }
                }

                SelectedIndex = _hazard.FindIndex(h => h == hazard) + _weapon.Count;

                return count < 2;
            }

            return false;
        }
    }
}