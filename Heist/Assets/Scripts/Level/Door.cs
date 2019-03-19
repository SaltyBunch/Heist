using System;
using Character;
using Game;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace Level
{
    public class Door : MonoBehaviour
    {
        [SerializeField] internal Light _light;
        [SerializeField] private bool _locked = false;

        [SerializeField] internal Color _lockedColor;

        [SerializeField] internal Animator _anim;

        [SerializeField] internal bool _open;

        internal bool _animating = false;
        
        private PlayerControl _player;
        private LockQuickTimeEvent _quickTime;
        [SerializeField] internal Color _unlockedColor;

        [SerializeField] private LockQuickTimeEvent _lockQuickTimeEvent;

        public bool Locked
        {
            get => _locked;
            set
            {
                if (_locked != value && _light != null) _light.color = value ? _lockedColor : _unlockedColor;
                _locked = value;
            }
        }


        private void Reset()
        {
            gameObject.tag = "Door";
        }

        public virtual void Open(PlayerControl player)
        {
            if (_locked)
            {
                _quickTime = player.BaseCharacter.InitializeQuickTime(_lockQuickTimeEvent) as LockQuickTimeEvent;
                _quickTime.QuickTimeType = QuickTimeEvent.Type.GoldPile;
                _quickTime.Events += QuickTimeEventMonitor;
                _player = player;
                _player.OnMoveCancel += PlayerOnOnMoveCancel;
            }
            else if (!_animating)
            {
                if (!_open)
                {
                    _animating = true;
                    _anim.SetTrigger("Open");
                }
                else
                {
                    _animating = true;
                    _anim.SetTrigger("Close");
                }
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

        void SetOpen()
        {
            _animating = false;
            _open = true;
        }

        void SetClose()
        {
            _animating = false;
            _open = false;
        }
    }


    //TODO Coroutine for close
}