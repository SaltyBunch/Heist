using System;
using System.Collections.Generic;
using Character;
using Game;
using UnityEngine;

namespace Level
{
    public class Vault : MonoBehaviour
    {
        public delegate void OpenDoor();

        private static readonly int Open = Animator.StringToHash("Open");

        [SerializeField] private Animator _anim;

        [SerializeField] private AudioSource _audioSource;

        private bool _closed = true;

        [SerializeField] private Dictionary<KeyType, bool> _keys = new Dictionary<KeyType, bool>
        {
            {KeyType.YellowKey, false}, {KeyType.RedKey, false}
        };

        [SerializeField] private AudioClip _openClip;

        [SerializeField] private Light _redLight;
        [SerializeField] private Light _yellowLight;

        public void OpenVault()
        {
            if (_keys[KeyType.RedKey] && _keys[KeyType.YellowKey] && _closed)
            {
                StartCoroutine(LevelManager.LevelManagerRef.OpenVault(OpenDoorMethod));
                _audioSource.clip = _openClip;
                _audioSource.Play();
                _closed = false;
            }
        }

        public void UseKey(Dictionary<KeyType, bool> keys, Player player)
        {
            foreach (var key in (KeyType[]) Enum.GetValues(typeof(KeyType)))
                if (key != KeyType.BlueKey && keys[key])
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

            if (!_keys[KeyType.YellowKey] && !_keys[KeyType.RedKey])
                player.PlayerUiManager.NeedsBothKeys();
            else if (!_keys[KeyType.YellowKey])
                player.PlayerUiManager.NeedKey(KeyType.YellowKey);
            else if (!_keys[KeyType.RedKey]) player.PlayerUiManager.NeedKey(KeyType.RedKey);
            OpenVault();
        }

        private void OpenDoorMethod()
        {
            _anim.SetTrigger(Open);
            GameManager.SetLayerOnAll(gameObject, 10);
        }
    }
}