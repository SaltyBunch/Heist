using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private List<Sprite> _loadingScreens;

        [SerializeField] private Slider _slider;

        public float Progress
        {
            set => _slider.value = value / 0.9f;
        }

        private void OnEnable()
        {
            _image.sprite = _loadingScreens[0];
        }

        public void Next()
        {
            if (_loadingScreens.Count > 1) _image.sprite = _loadingScreens[1];
        }
    }
}