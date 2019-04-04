using System;
using Character;
using Game;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Level
{
    public class Door : MonoBehaviour
    {
        [SerializeField] internal Animator _anim;

        internal bool _animating;

        [SerializeField] internal AudioSource _audioSource;
        [SerializeField] internal AudioClip _closeClip;
        private bool _interacting;
        [SerializeField] internal Light _light;
        [SerializeField] private bool _locked;

        [SerializeField] internal Color _lockedColor;

        [SerializeField] private LockQuickTimeEvent _lockQuickTimeEvent;

        [SerializeField] internal bool _open;

        [SerializeField] internal AudioClip _openClip;

        private PlayerControl _player;
        private LockQuickTimeEvent _quickTime;
        [SerializeField] internal Color _unlockedColor;

        public bool IsOpen => _animating ? !_open : _open;

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

        private void SetOpen()
        {
            _animating = false;
            _open = true;
        }

        private void SetClose()
        {
            _animating = false;
            _open = false;
        }
    }
}