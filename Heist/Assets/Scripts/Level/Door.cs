using UnityEngine;

namespace Level
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Light _light;
        [SerializeField] private bool _locked;

        [SerializeField] private Color _lockedColor;
        [SerializeField] private Color _unlockedColor;

        private void Reset()
        {
            gameObject.tag = "Door";
        }

        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value) _light.color = value ? _lockedColor : _unlockedColor;
                _locked = value;
            }
        }
    }
}