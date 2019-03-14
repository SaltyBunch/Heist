using Rewired;
using UnityEngine;

namespace Game
{
    public class LockQuickTimeEvent : QuickTimeEvent
    {
        private Button[] _buttons;

        private Input _controlInput;
        private int _index;
        private Player _player;

        public Type QuickTimeType;

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
                //todo update ui
                _index++;
                if (_index == _buttons.Length)
                    Destroy(gameObject, 0.2f);
            }
            else
            {
                //failure
                Events?.Invoke(this, new QuickTimeEventArgs
                {
                    Result = false, State = _index, Type = QuickTimeType
                });
                Generate();
                //todo update ui
            }
        }

        public void Generate()
        {
            _buttons = new Button[4];
            for (var i = 0; i < _buttons.Length; i++) _buttons[i] = (Button) Random.Range(0, 4);

            _index = 0;
        }


        public void Update()
        {
            ControlInput = new Input
            {
                A = _player.GetButton("QuickTimeA"),
                B = _player.GetButton("QuickTimeB"),
                X = _player.GetButton("QuickTimeX"),
                Y = _player.GetButton("QuickTimeY")
            };
        }

        public event QuickTimeEventHandler Events;

        private struct Input
        {
            public bool A;
            public bool B;
            public bool X;
            public bool Y;
        }
    }
}