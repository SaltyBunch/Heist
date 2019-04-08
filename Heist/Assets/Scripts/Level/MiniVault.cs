using System;
using System.Collections.Generic;
using Character;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Level
{
    public enum SpawnObjects
    {
        ElectricHazard,
        LaserHazard,
        StunGun,
        Baton,
        Gold,
        BlueKey,
        RedKey,
        YellowKey,
        Shotgun,
    }

    public class MiniVault : MonoBehaviour
    {
        [SerializeField] private List<SpawnObjects> _possibleSpawns;

        [SerializeField] private List<Pickup.Pickup> _pickupPrefabs;

        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _openClip;

        private void Reset()
        {
            gameObject.tag = "MiniVault";
        }

        private PlayerControl _player;

        private bool _interacting;

        [SerializeField] private Animator _anim;

        private LockQuickTimeEvent _quickTime;
        [SerializeField] private LockQuickTimeEvent _lockQuickTimeEvent;

        public void StartChanneling(PlayerControl player)
        {
            if (_interacting) return;
            _interacting = true;
            _quickTime = player.BaseCharacter.InitializeQuickTime(_lockQuickTimeEvent) as LockQuickTimeEvent;
            _quickTime.QuickTimeType = QuickTimeEvent.Type.MiniVault;
            _quickTime.Events += QuickTimeEventMonitor;
            _quickTime.Generate(player, transform.position);
            _player = player;
            _player.OnMoveCancel += CancelChannel;
        }

        public void CancelChannel(object sender, EventArgs e)
        {
            _interacting = false;
            _quickTime.Events -= QuickTimeEventMonitor;
            Destroy(_quickTime.gameObject, 0.2f);
            _quickTime = null;
            _player.OnMoveCancel -= CancelChannel;
            _player = null;
        }

        private void QuickTimeEventMonitor(Object sender, QuickTimeEvent.QuickTimeEventArgs e)
        {
            if (e.Result)
            {
            }

            if (e.Complete)
            {
                _anim.SetTrigger("Open");
                if (_openClip != null)
                {
                    _audioSource.clip = _openClip;
                    _audioSource.Play();
                }

                SpawnObjects();
                CancelChannel(sender, e);
            }
        }

        private void SpawnObjects()
        {
            var i = Random.Range(0, _possibleSpawns.Count);

            Instantiate(_pickupPrefabs[(int) _possibleSpawns[i]], _player.transform.position, Quaternion.identity,
                null);

            this.gameObject.tag = "Untagged";
        }
    }
}