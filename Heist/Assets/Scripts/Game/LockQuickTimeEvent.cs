using System.Linq;
using Character;
using Rewired;
using UnityEngine;
using UnityEngine.UI;
using Player = Rewired.Player;

namespace Game
{
    public class LockQuickTimeEvent : QuickTimeEvent
    {
        [SerializeField] private Sprite[] _spriteButtons;
        [SerializeField] private Sprite[] _spriteStatus;

        [SerializeField] private Image[] _buttonRenderer;
        [SerializeField] private Image[] _statusRenderer;

        [SerializeField] private Animator _animator;

        private Button[] _buttons;

        private Input _controlInput;
        private int _index;
        private Player _player;
        private Vector3 _position;

        public Type QuickTimeType;
        private static readonly int Unlock = Animator.StringToHash("Unlock");

        private Input ControlInput
        {
            get => _controlInput;
            set
            {
                if (!Equals(value, _controlInput))
                {
                    if (value.A && !_controlInput.A) PressButton(Button.A);
                    if (value.B && !_controlInput.B) PressButton(Button.B);
                    if (value.X && !_controlInput.X) PressButton(Button.X);
                    if (value.Y && !_controlInput.Y) PressButton(Button.Y);
                }

                _controlInput = value;
            }
        }

        private void PressButton(Button button)
        {
            if (button == _buttons[_index])
            {
                //success
                Events?.Invoke(this, new QuickTimeEventArgs
                {
                    Result = true, State = _index, Type = QuickTimeType, Complete = _index + 1 == _buttons.Length
                });
                _statusRenderer[_index].sprite = _spriteStatus[0];

                _index++;
                if (_index == _buttons.Length)
                {
                    _animator.SetTrigger(Unlock);
                    Destroy(gameObject, 0.954918f);
                }
            }
            else
            {
                //failure
                Events?.Invoke(this, new QuickTimeEventArgs
                {
                    Result = false, State = _index, Type = QuickTimeType
                });
                LevelManager.LevelManagerRef.Notify(_position, NotifyType.TripTrap);
                Generate();
            }
        }


        public void Generate(PlayerControl player, Vector3 position)
        {
            _player = player.Player;
            Generate();
        }

        public void Generate()
        {
            if (_player == null) _player = ReInput.players.Players.First();
            _buttons = new Button[4];
            for (var i = 0; i < _buttons.Length; i++) _buttons[i] = (Button) Random.Range(0, 4);

            //put buttons on sprite
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttonRenderer[i].sprite = _spriteButtons[(int) _buttons[i]];
                _statusRenderer[i].sprite = _spriteStatus[1];
            }

            _index = 0;
        }


        public void Update()
        {
            if (_player != null)
            {
                ControlInput = new Input
                {
                    A = _player.GetButton("QuickTimeA"),
                    B = _player.GetButton("QuickTimeB"),
                    X = _player.GetButton("QuickTimeX"),
                    Y = _player.GetButton("QuickTimeY")
                };
            }
        }

        public event QuickTimeEventHandler Events;

        private struct Input
        {
            public bool A;
            public bool B;
            public bool X;
            public bool Y;
        }

        public void Clear()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttonRenderer[i].sprite = null;
                _statusRenderer[i].sprite = null;
            }
        }
    }
}