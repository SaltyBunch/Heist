using System;
using Character;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Level
{
    public class Door : MonoBehaviour
    {
        [SerializeField] private Light _light;
        [SerializeField] private bool _locked;

        [SerializeField] private Color _lockedColor;
        [SerializeField] private Animation.Door _door;
 
        private bool _open;

        private PlayerControl _player;
        private LockQuickTimeEvent _quickTime;
        [SerializeField] private Color _unlockedColor;

        [SerializeField] private LockQuickTimeEvent lockQuickTimeEvent;

        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value) _light.color = value ? _lockedColor : _unlockedColor;
                _locked = value;
            }
        }

        private void Reset()
        {
            gameObject.tag = "Door";
        }

        public void Open(PlayerControl player)
        {
            if (_locked)
            {
                _quickTime = Instantiate(lockQuickTimeEvent);
                _quickTime.QuickTimeType = QuickTimeEvent.Type.GoldPile;
                _quickTime.Events += QuickTimeEventMonitor;
                _player = player;
                _player.OnMoveCancel += PlayerOnOnMoveCancel;
            }
            else //todo open door
            {
                _open = true;
                _door.Open();
            }
        }

        private void PlayerOnOnMoveCancel(object sender, EventArgs e)
        {
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
                _locked = false;
                Open(_player);
                _quickTime.Events -= QuickTimeEventMonitor;
                _quickTime = null;
                _player.OnMoveCancel -= PlayerOnOnMoveCancel;
                _player = null;
            }
        }
    }


    //TODO Coroutine for close
}