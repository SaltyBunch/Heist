using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Level
{
    public class Vault : MonoBehaviour
    {
        public delegate void OpenDoor();

        [SerializeField] private Dictionary<KeyType, bool> _keys = new Dictionary<KeyType, bool>
        {
            {KeyType.YellowKey, false}, {KeyType.RedKey, false}
        };

        private bool _closed = true;

        public bool OpenVault()
        {
            if (_keys[KeyType.RedKey] && _keys[KeyType.YellowKey] && _closed)
            {
                StartCoroutine(LevelManager.LevelManagerRef.OpenVault(OpenDoorMethod));
                _closed = false;
            }

            return !_closed;
        }

        public void UseKey(KeyType key)
        {
            _keys[key] = true;
        }

        private void OpenDoorMethod()
        {
            //call animation on door
        }
    }
}