using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Level
{
    public class Vault : MonoBehaviour
    {
        public delegate void OpenDoor();

        [SerializeField] private Animator _anim;

        [SerializeField] private Light _redLight;
        [SerializeField] private Light _yellowLight;

        [SerializeField] private AudioSource _audioSource;

        [SerializeField] private AudioClip _openClip;

        private bool _closed = true;

        [SerializeField] private Dictionary<KeyType, bool> _keys = new Dictionary<KeyType, bool>
        {
            {KeyType.YellowKey, false}, {KeyType.RedKey, false}
        };

        private static readonly int Open = Animator.StringToHash("Open");

        public bool OpenVault()
        {
            if (_keys[KeyType.RedKey] && _keys[KeyType.YellowKey] && _closed)
            {
                StartCoroutine(LevelManager.LevelManagerRef.OpenVault(OpenDoorMethod));
                _audioSource.clip = _openClip;
                _audioSource.Play();
                _closed = false;
            }

            return !_closed;
        }

        public void UseKey(Dictionary<KeyType, bool> keys)
        {
            foreach (var key in (KeyType[]) Enum.GetValues(typeof(KeyType)))
                if (keys[key] && key != KeyType.BlueKey)
                {
                    _keys[key] = true;
                    switch (key)
                    {
                        case KeyType.RedKey:
                            _redLight.enabled = false;
                            break;
                        case KeyType.YellowKey:
                            _yellowLight.enabled = false;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }


            OpenVault();
        }

        private void OpenDoorMethod()
        {
            _anim.SetTrigger(Open);
        }
    }
}