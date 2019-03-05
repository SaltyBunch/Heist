using System;
using Character;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Level
{
    public class MiniVault : MonoBehaviour
    {
        [SerializeField] private Pickup.Pickup[] _pickups;

        private void Reset()
        {
            gameObject.tag = "MiniVault";
        }

        private PlayerControl _player;

        private bool _interacting;

        private LockQuickTimeEvent _quickTime;
        [SerializeField] private LockQuickTimeEvent _lockQuickTimeEvent;
        [SerializeField] private Vector3 _pickupSpawn;
        [SerializeField] private Floor _floor;

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
                SpawnObjects();
                CancelChannel(sender, e);
            }
        }

        private void SpawnObjects()
        {
            foreach (var pickup in _pickups)
            {
                var pickupGO = Instantiate(pickup,_player.transform.position, Quaternion.identity);
                GameManager.SetLayerOnAll(pickupGO.gameObject, LevelManager.PickupMask[_floor]);
            }
        }
    }
}