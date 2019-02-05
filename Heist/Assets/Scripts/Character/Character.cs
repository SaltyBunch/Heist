using System;
using System.Collections;
using UnityEngine;

namespace Character
{
    [Serializable]
    public struct Stats
    {
        public int Health;
        public int Speed;
        public int Dexterity;
    }

    public delegate void HealthUpdatedEventHandler(object sender, HealthChangedEventArgs e);

    public class HealthChangedEventArgs
    {
        public int Health;
    }

    public class Character : MonoBehaviour
    {
        private bool _damageCooldown;
        private bool _firstDamage;
        private int _stacks;
        private bool _stun;
        private bool _stunCooldown;

        public event HealthUpdatedEventHandler HealthChanged;

        private float _timeSinceDamage;
        [SerializeField] public Stats Stats;

        internal int timesStunned;
        private bool _invincible;
        [SerializeField, Range(0.01f, 1)] private float _invFrames = 0.333f;

        public int Stacks
        {
            get => _stacks;
            set
            {
                if (value > _stacks && !_invincible && !(_stun || _stunCooldown))
                {
                    _firstDamage = true;
                    _timeSinceDamage = 0;
                    _stacks = value;
                    if (_stacks >= Stats.Health)
                    {
                        StartCoroutine(Stun());
                    }
                    StartCoroutine(IFrames());
                }
                else if (value < _stacks)
                {
                    _stacks = value;
                    if (_stacks < 0)
                        _stacks = 0;
                }

                if (HealthChanged != null)
                    HealthChanged(this, new HealthChangedEventArgs() {Health = Stats.Health - _stacks});
            }
        }

        internal virtual void Update()
        {
            //Stack decreasing
            _timeSinceDamage += Time.deltaTime;
            if (_firstDamage && _stacks > 0)
            {
                //wait for 20 sec
                if (_timeSinceDamage >= 20)
                {
                    Stacks -= 1;
                    _timeSinceDamage = 0;
                    _firstDamage = false;
                }
            }
            else if (_stacks > 0)
            {
                //wait for 5 secs
                if (_timeSinceDamage >= 5)
                {
                    Stacks -= 1;
                    _timeSinceDamage = 0;
                }
            }
        }

        private IEnumerator Stun()
        {
            _stun = true;
            timesStunned += 1;
            yield return new WaitForSeconds(5);
            _stun = false;
            _stunCooldown = true;
            yield return new WaitForSeconds(3);
            _stunCooldown = false;
        }

        private IEnumerator IFrames()
        {
            _invincible = true;
            yield return new WaitForSeconds(_invFrames);
            _invincible = false;
        }
    }
}