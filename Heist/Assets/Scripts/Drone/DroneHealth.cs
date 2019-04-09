using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public class DroneHealth : MonoBehaviour
    {
        [SerializeField] private List<SpriteRenderer> _healthBars;
        [SerializeField] private GameObject _health;
        [SerializeField, Range(0, 5)] private float _healthDelayTime = 2;
        private float _timer;

        public void SetHealth(int amount)
        {
            for (var index = 0; index < _healthBars.Count; index++)
            {
                var healthBar = _healthBars[index];
                if (index < amount)
                {
                    healthBar.gameObject.SetActive(true);
                }
                else
                {
                    healthBar.gameObject.SetActive(false);
                }
            }

            _timer = _healthDelayTime;

            _health.SetActive(true);
        }

        private void Update()
        {
            if (_timer > 0)
            {
                _timer -= Time.deltaTime;
            }
            else
            {
                _health.SetActive(false);
            }

            _health.transform.rotation = Quaternion.identity;
        }
    }
}