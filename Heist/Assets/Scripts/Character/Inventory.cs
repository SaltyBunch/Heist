using UnityEngine;

namespace Character
{
    public class Inventory : MonoBehaviour
    {
        private readonly Hazard.Hazard[] _hazard = new Hazard.Hazard[3];
        private int _selectedHazard;
        private int _selectedWeapon;

        private readonly Weapon.Weapon[] _weapon = new Weapon.Weapon[2];
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

        internal Hazard.Hazard Hazard
        {
            get => _hazard[_selectedHazard];
            set
            {
                foreach (var haz in _hazard)
                    if (haz != null && value != null && haz.GetType() == value.GetType())
                        return;

                //destroy current hazard
                if (value == null) Destroy(_hazard[_selectedHazard].gameObject);
                //set _hazard to value
                _hazard[_selectedHazard] = value;
                //bind value to visual
                if (_hazard[_selectedHazard] == null) return;

                for (var i = 0; i < _hazard.Length; i++)
                {
                    if (_hazard[i] != null) continue;
                    _selectedHazard = i;
                    break;
                }


                var hazardTrans = _hazard[_selectedHazard].transform;
                hazardTrans.parent = transform;
                hazardTrans.localPosition = Vector3.zero;
                _hazard[_selectedHazard].Bind(gameObject);
            }
        }

        internal Weapon.Weapon Weapon
        {
            get => _weapon[_selectedWeapon];
            set
            {
                foreach (var haz in _weapon)
                    if (haz != null && value != null && haz.GetType() == value.GetType())
                        return;

                //destroy current hazard
                if (value == null) Destroy(_weapon[_selectedWeapon].gameObject);
                //set _hazard to value
                _weapon[_selectedWeapon] = value;
                //bind value to visual
                if (_weapon[_selectedWeapon] == null) return;

                for (var i = 0; i < _weapon.Length; i++)
                {
                    if (_weapon[i] != null) continue;
                    _selectedWeapon = i;
                    break;
                }

                var weaponTrans = _weapon[_selectedWeapon].transform;
                weaponTrans.parent = transform;
                weaponTrans.localPosition = Vector3.zero;
                _weapon[_selectedWeapon].Bind(gameObject);
            }
        }
    }
}