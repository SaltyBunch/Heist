using Rewired.Data.Mapping;
using UnityEngine;

namespace Game
{
    public class BarQuickTimeEvent : QuickTimeEvent
    {
        private struct Input
        {
            public bool A;
        }

        private Input _controlInput;

        private Input ControlInput
        {
            get => _controlInput;
            set
            {
                if (!Equals(value, _controlInput))
                {
                    if (value.A && !_controlInput.A) PressButton(Button.A);
                }

                _controlInput = value;
            }
        }

        private void PressButton(Button button)
        {
            if (button == Button.A && _index > _range - _bias && _index < _range + _bias)
            {
                //success
                Events(this, new QuickTimeEventArgs()
                {
                    Result = true, State = (int) (_index * 100), Type = QuickTimeType
                });
                //todo update ui
                _index++;
            }
            else
            {
                //failure
                Events(this, new QuickTimeEventArgs()
                {
                    Result = false, State = (int) (_index * 100), Type = QuickTimeType
                });
                Generate();
                //todo update ui
            }
        }

        private float _range;
        private Rewired.Player _player;
        private float _index;

        public void Generate()
        {
            _dir = 1;
            _index = 0;
        }


        public void Update()
        {
            ControlInput = new Input()
            {
                A = _player.GetButton("QuickTimeA"),
            };
            //pingpong on _index
            _index += Time.deltaTime * _dir;
            if (_index > 1)
                _dir *= -1;
        }

        public event QuickTimeEventHandler Events;

        public Type QuickTimeType;
        private int _dir;
        private float _bias = 0.05f;
    }
}