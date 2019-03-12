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
    }
}