using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Character
{
    public delegate void SelectionChangedEventHandler(object sender, SelectionChangedEventArgs e);

    public class SelectionChangedEventArgs
    {
        public Game.Item.Type Type;
        public Game.Item Item;
        public int Count;
    }


    public class Inventory : MonoBehaviour
    {
        private List<Hazard.Hazard> _hazard = new List<Hazard.Hazard>();
        private int _selectedHazard;
        private int _selectedWeapon;

        private int _selectedIndex;


        public event SelectionChangedEventHandler SelectionChanged;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value % (_hazard.Count + _weapon.Count);
                var selected = _selectedIndex >= _weapon.Count
                    ? (Game.Item) _hazard[_selectedIndex % _weapon.Count]
                    : (Game.Item) _weapon[_selectedIndex];

                var type = (selected is Weapon.Weapon ? Game.Item.Type.Weapon : Game.Item.Type.Hazard);
                SelectionChanged?.Invoke(this,
                    new SelectionChangedEventArgs()
                    {
                        Item = selected,
                        Type = type,
                        Count = type == Game.Item.Type.Weapon ? 1 : _hazard.Count(h => h == selected) //todo better
                    });
            }
        }

        private List<Weapon.Weapon> _weapon = new List<Weapon.Weapon>();
        public int GoldAmount;

        [SerializeField] private PlayerControl _player;

        private void Awake()
        {
            _player = GetComponent<PlayerControl>();
        }

        private void Reset()
        {
            _player = GetComponent<PlayerControl>();
        }

        public bool Add(Game.Item item)
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
                else _weapon.Add(weapon);
                SelectedIndex = _weapon.Count - 1;

                return true;
            }
            else if (item is Hazard.Hazard hazard)
            {
                var count = _hazard.Count(h => h == hazard);
                if (count < 2)
                {
                    _hazard.Add(hazard);
                }
                SelectedIndex = _hazard.FindIndex(h => h == hazard) + _weapon.Count;
                
                return count < 2;
            }

            return false;
        }
    }
}