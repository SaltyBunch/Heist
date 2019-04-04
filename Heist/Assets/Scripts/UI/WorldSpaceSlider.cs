using System;
using Game;
using UnityEngine;

namespace UI
{
    public class WorldSpaceSlider : MonoBehaviour
    {
        public enum AudioSource
        {
            Master,
            SFX,
            Music
        }

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private Transform _left;
        private MasterMixer _masterMixer;
        [SerializeField] private float _maxValue;

        [SerializeField] private float _minValue;
        [SerializeField] private Transform _right;

        [SerializeField] private SpriteRenderer _slider;
        private float _value;
        public float Step;

        public float Value
        {
            get => _value;
            set
            {
                _value = Mathf.Clamp(value, _minValue, _maxValue);
                switch (_audioSource)
                {
                    case AudioSource.Master:
                        _masterMixer.UpdateMasterVolume(_value);
                        break;
                    case AudioSource.SFX:
                        _masterMixer.UpdateSFXVolume(_value);
                        break;
                    case AudioSource.Music:
                        _masterMixer.UpdateMusicVolume(_value);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                _slider.transform.localPosition =
                    (_right.transform.localPosition - _left.transform.localPosition) *
                    ((_value - _minValue) / (_maxValue - _minValue)) + _left.transform.localPosition;
            }
        }

        public bool Selected
        {
            set => _slider.color = value ? new Color(0, 0.5f, 0.5f) : Color.white;
        }

        private void Start()
        {
            _masterMixer = GameManager.GameManagerRef.MasterMixer;
            switch (_audioSource)
            {
                case AudioSource.Master:
                    Value = _masterMixer.GetMasterVolume();
                    break;
                case AudioSource.SFX:
                    Value = _masterMixer.GetSFXVolume();
                    break;
                case AudioSource.Music:
                    Value = _masterMixer.GetMusicVolume();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}