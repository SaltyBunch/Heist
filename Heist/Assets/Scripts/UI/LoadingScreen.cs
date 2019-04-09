using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

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
        [SerializeField] private RawImage _rawImage;
        [SerializeField] private List<Sprite> _loadingScreens;
        [SerializeField] private List<Sprite> _loadingBackdrops;
        private bool _atVideo = false;
        private bool _skip;
        private bool[] _skippers;
        private int _numSkip;
        [SerializeField] private TextMeshProUGUI _skipText;
        [SerializeField] private TextMeshProUGUI _skipTotal;

        [SerializeField] private VideoPlayer _videoPlayer;
        private int _index = 0;
        private bool _played;

        private void OnEnable()
        {
            _image.sprite = _loadingBackdrops[Random.Range(0, _loadingBackdrops.Count)];
            _slider.gameObject.SetActive(true);
        }

        public void Next()
        {
            _slider.gameObject.SetActive(false);
            _skipText.gameObject.SetActive(true);
            NumSkip = 0;
            Skippers = new bool[GameManager.NumPlayers];
            _skipTotal.text = GameManager.NumPlayers.ToString();
            _skip = true;
            Time.timeScale = 0;
            _videoPlayer.gameObject.SetActive(true);
            _atVideo = true;
        }

        private void EndLoading()
        {
            Time.timeScale = 1;
            _skip = false;
            gameObject.SetActive(false);
            _played = false;
            _index = 0;
            _atVideo = false;
            _skipText.gameObject.SetActive(false);
            _slider.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (_skip)
            {
                if (!_videoPlayer.isPlaying && _atVideo)
                {
                    if (_played)
                    {
                        _videoPlayer.gameObject.SetActive(false);
                        _image.sprite = _loadingScreens[_index];
                        _atVideo = false;
                        Skippers = new bool[GameManager.NumPlayers];
                    }
                    else
                    {
                        _videoPlayer.Play();
                    }
                }
                else if (_videoPlayer.isPlaying && !_atVideo)
                {
                    _videoPlayer.Stop();
                }
                else if (_videoPlayer.isPlaying)
                {
                    _played = true;
                }

                for (var i = 0; i < ReInput.players.Players.Count; i++)
                {
                    var player = ReInput.players.Players[i];
                    if (player.GetButtonDown("UISubmit"))
                    {
                        Skippers[i] = true;
                    }
                }

                NumSkip = _skippers.Count(x => x);
                if (NumSkip == _skippers.Length)
                {
                    if (_atVideo)
                    {
                        _videoPlayer.gameObject.SetActive(false);
                        _image.sprite = _loadingScreens[_index];
                        _atVideo = false;
                        Skippers = new bool[GameManager.NumPlayers];
                    }
                    else
                    {
                        Skippers = new bool[GameManager.NumPlayers];
                        _index++;
                        if (_index == _loadingScreens.Count)
                            EndLoading();
                    }
                }
            }
        }
    }
}