using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public float Progress
        {
            set => _slider.value = value / 0.9f;
        }

        public bool[] Skippers
        {
            get { return _skippers; }
            set { _skippers = value; }
        }

        public int NumSkip
        {
            get { return _numSkip; }
            set
            {
                _numSkip = value;
                _skipText.text = _numSkip.ToString();
            }
        }

        [SerializeField] private Slider _slider;
        [SerializeField] private Image _image;
        [SerializeField] private List<Sprite> _loadingScreens;
        private bool _skip;
        private bool[] _skippers;
        private int _numSkip;
        [SerializeField] private TextMeshProUGUI _skipText;
        [SerializeField] private TextMeshProUGUI _skipTotal;

        private void OnEnable()
        {
            _image.sprite = _loadingScreens[0];
            _slider.gameObject.SetActive(true);
        }

        public IEnumerator Next()
        {
            _slider.gameObject.SetActive(false);
            _skipText.gameObject.SetActive(true);
            NumSkip = 0;
            Skippers = new bool[GameManager.NumPlayers];
            _skipTotal.text = GameManager.NumPlayers.ToString();
            _skip = true;
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(8f);
            if (_loadingScreens.Count > 1)
            {
                _image.sprite = _loadingScreens[1];
            }

            yield return new WaitForSecondsRealtime(7f);
            EndLoading();
        }

        private void EndLoading()
        {
            Time.timeScale = 1;
            _skip = false;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            if (_skip)
            {
                for (var i = 0; i < ReInput.players.Players.Count; i++)
                {
                    var player = ReInput.players.Players[i];
                    if (player.GetButtonDown("UISubmit"))
                    {
                        Skippers[i] = true;
                    }
                }

                NumSkip = _skippers.Count(x => x);
                if (NumSkip == _skippers.Length) EndLoading();
            }
        }
    }
}