using System;
using System.Collections;
using Character;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game
{
    public class BarQuickTimeEvent : QuickTimeEvent
    {
        [SerializeField] private AudioSource _audioSource;

        private Input _controlInput;
        private int _dir;
        [SerializeField] private Image _greenArea;
        private float _index;
        private PlayerControl _player;

        [SerializeField] private Image _pointer;

        private float _range;
        private bool _started;

        [SerializeField] private AudioClip _sucess, _failure;
        private float _timer = 5;

        public Type QuickTimeType;

        private Input ControlInput
        {
            get => _controlInput;
            set
            {
                if (!Equals(value, _controlInput))
                {
                    if (value.A && !_controlInput.A)
                        PressButton(Button.A);
                    else if (value.B && !_controlInput.B)
                        PressButton(Button.B);
                    else if (value.X && !_controlInput.X)
                        PressButton(Button.X);
                    else if (value.Y && !_controlInput.Y)
                        PressButton(Button.Y);
                }

                _controlInput = value;
            }
        }

        private void PressButton(Button button)
        {
            //_timer = 5;
            if ((button == Button.A || button == Button.B || button == Button.X || button == Button.Y) && CheckIndex())
            {
                //success
                Events?.Invoke(this, new QuickTimeEventArgs
                {
                    Result = true,
                    State = (int) (_index * 100),
                    Type = QuickTimeType
                });
                _audioSource.clip = _sucess;
                _audioSource.Play();
                StartCoroutine(FlashColour(Color.green));
            }
            else
            {
                //failure
                Events?.Invoke(this, new QuickTimeEventArgs
                {
                    Result = false,
                    State = (int) (_index * 100),
                    Type = QuickTimeType
                });
                _audioSource.clip = _failure;
                _audioSource.Play();
                StartCoroutine(FlashColour(Color.red));
            }
        }

        private IEnumerator FlashColour(Color color)
        {
            _pointer.color = color;
            yield return new WaitForSeconds(0.1f);
            _pointer.color = Color.white;
            Generate();
        }

        public void Generate(PlayerControl player)
        {
            _player = player;
            Generate();
        }

        public void Generate()
        {
            //_timer = 5;
            _started = true;

            _greenArea.fillOrigin = (Random.Range(1, 4) + _greenArea.fillOrigin) % 4;
            _greenArea.fillAmount = _range;
            _greenArea.fillClockwise = false;

            _dir = 1;
        }

        private bool CheckIndex()
        {
            var point = _greenArea.fillOrigin - 2; //top == 0, bottom == -2, right == -1, left == 1

            float startAngle = 0;
            switch (point)
            {
                case -2:
                    startAngle = 0.5f;
                    break;
                case -1:
                    startAngle = 0.75f;
                    break;
                case 0:
                    startAngle = 0;
                    break;
                case 1:
                    startAngle = 0.25f;
                    break;
            }

            float origin;

            if (_greenArea.fillClockwise)
                origin = (startAngle - _range / 2) % 1;
            else
                origin = (startAngle + _range / 2) % 1;

            if (Math.Abs(_index - origin) < _range || Math.Abs(1 - _index - origin) < _range) return true;

            return false;
        }

        public void SetDexterity(int dexterity)
        {
            switch (dexterity)
            {
                case 2:
                    _range = 0.1f;
                    break;
                case 3:
                    _range = 0.15f;
                    break;
                case 4:
                    _range = 0.2f;
                    break;
                case 5:
                    _range = 0.25f;
                    break;
            }
        }

        public void Update()
        {
            if (_player != null)
                ControlInput = new Input
                {
                    A = _player.Control.QuickTimeA,
                    B = _player.Control.QuickTimeB,
                    X = _player.Control.QuickTimeX,
                    Y = _player.Control.QuickTimeY
                };

            if (_started) _timer -= Time.deltaTime;
            if (_started && _timer <= 0)
                Events?.Invoke(this, new QuickTimeEventArgs
                {
                    Result = false,
                    State = -1,
                    Type = QuickTimeType
                });

            _index += Time.deltaTime * _dir;

            if (_index > 1) _index %= 1;
            _pointer.transform.localRotation = Quaternion.Euler(0, 0, _index * 360);
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