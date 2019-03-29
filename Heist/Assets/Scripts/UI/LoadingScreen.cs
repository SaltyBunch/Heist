using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LoadingScreen : MonoBehaviour
    {
        public float Progress
        {
            set => _slider.value = value/0.9f;
        }

        [SerializeField] private Slider _slider;
        [SerializeField] private Image _image;
        [SerializeField] private List<Sprite> _loadingScreens;

        private void OnEnable()
        {
            _image.sprite = _loadingScreens[Random.Range(0, _loadingScreens.Count)];
        }
    }
}