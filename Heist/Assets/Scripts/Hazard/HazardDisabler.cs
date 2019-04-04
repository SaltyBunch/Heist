using System;
using Character;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hazard
{
    public class HazardDisabler : MonoBehaviour
    {
        [SerializeField] private Hazard _hazard;
        private bool _interacting;
        [SerializeField] private LockQuickTimeEvent _lockQuickTimeEvent;
        private PlayerControl _player;
        private LockQuickTimeEvent _quickTime;

        public void DisableHazard(PlayerControl player)
        {
            if (_interacting) return;
            _interacting = true;
            _quickTime = player.BaseCharacter.InitializeQuickTime(_lockQuickTimeEvent) as LockQuickTimeEvent;
            _quickTime.QuickTimeType = QuickTimeEvent.Type.Door;
            _quickTime.Events += QuickTimeEventMonitor;
            _quickTime.Generate(player, transform.position);
            _player = player;
            _player.OnMoveCancel += PlayerOnOnMoveCancel;
        }

        private void PlayerOnOnMoveCancel(object sender, EventArgs e)
        {
            _interacting = false;
            _quickTime.Events -= QuickTimeEventMonitor;
            Destroy(_quickTime.gameObject, 0.2f);
            _quickTime = null;
            _player.OnMoveCancel -= PlayerOnOnMoveCancel;
            _player = null;
        }

        private void QuickTimeEventMonitor(Object sender, QuickTimeEvent.QuickTimeEventArgs e)
        {
            if (e.Complete)
            {
                _interacting = false;
                _quickTime.Events -= QuickTimeEventMonitor;
                _quickTime = null;
                _player.OnMoveCancel -= PlayerOnOnMoveCancel;
                _player = null;
                Destroy(_hazard.gameObject, 0.2f);
            }
        }
    }
}