using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Level
{
    public class Vault : MonoBehaviour
    {
        public delegate void OpenDoor();

        private bool _closed = true;

        [SerializeField] private Dictionary<KeyType, bool> _keys = new Dictionary<KeyType, bool>
        {
            {KeyType.YellowKey, false}, {KeyType.RedKey, false}
        };

        public bool OpenVault()
        {
            if (_keys[KeyType.RedKey] && _keys[KeyType.YellowKey] && _closed)
            {
                StartCoroutine(LevelManager.LevelManagerRef.OpenVault(OpenDoorMethod));
                _closed = false;
            }

            return !_closed;
        }

        public void UseKey(Dictionary<KeyType, bool> keys)
        {
            foreach (var key in (KeyType[]) Enum.GetValues(typeof(KeyType)))
                if (keys[key] && key != KeyType.BlueKey)
                    _keys[key] = true;

            OpenVault();
        }

        private void OpenDoorMethod()
        {
            //todo call animation on door
        }
    }
}