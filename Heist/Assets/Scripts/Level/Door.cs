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

        [SerializeField] internal AudioSource _audioSource;

        [SerializeField] internal AudioClip _openClip;
        [SerializeField] internal AudioClip _closeClip;

        public bool IsOpen => _animating ? !_open : _open;

        internal bool _animating = false;

        private PlayerControl _player;
        private LockQuickTimeEvent _quickTime;
        [SerializeField] internal Color _unlockedColor;

        [SerializeField] private LockQuickTimeEvent _lockQuickTimeEvent;
        private bool _interacting;

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
                if (_interacting) return;
                _interacting = true;
                _quickTime = player.BaseCharacter.InitializeQuickTime(_lockQuickTimeEvent) as LockQuickTimeEvent;
                _quickTime.QuickTimeType = QuickTimeEvent.Type.Door;
                _quickTime.Events += QuickTimeEventMonitor;
                _quickTime.Generate(player, transform.position);
                _player = player;
                _player.OnMoveCancel += PlayerOnOnMoveCancel;
            }
            else if (!_animating)
            {
                if (!_open)
                {
                    _animating = true;
                    _anim.SetTrigger("Open");
                    if (_openClip != null)
                    {
                        _audioSource.clip = _openClip;
                        _audioSource.Play();
                    }
                }
                else
                {
                    _animating = true;
                    _anim.SetTrigger("Close");
                    if (_closeClip != null)
                    {
                        _audioSource.clip = _closeClip;
                        _audioSource.Play();
                    }
                }
            }
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
}