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

    [RequireComponent(typeof(Rigidbody))]
    public class Character : MonoBehaviour
    {
        private bool _damageCooldown;
        private bool _firstDamage;
        [SerializeField] [Range(0.01f, 1)] private float _invFrames = 0.333f;
        private bool _invincible;
        [SerializeField] private float _knockbackForce;

        private Rigidbody _rgd;
        private int _stacks;
        private bool _stun;

        public bool Stunned => _stun;
        
        private bool _stunCooldown;
        
        private float _timeSinceDamage;
        [SerializeField] public Stats Stats;

        internal int timesStunned;

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
                    if (_stacks >= Stats.Health) StartCoroutine(Stun());

                    StartCoroutine(IFrames());
                }
                else if (value < _stacks)
                {
                    _stacks = value;
                    if (_stacks < 0)
                        _stacks = 0;
                }

                if (HealthChanged != null)
                    HealthChanged(this, new HealthChangedEventArgs {Health = Stats.Health - _stacks});
            }
        }

        public event HealthUpdatedEventHandler HealthChanged;


        private void Start()
        {
            if (_rgd == null)
                _rgd = GetComponent<Rigidbody>();
        }

        internal virtual void Update()
        {
            //Stack decreasing
            _timeSinceDamage += Time.deltaTime;
            if (_firstDamage && _stacks > 0)
            {
                //wait for 5 sec
                if (_timeSinceDamage >= 5)
                {
                    Stacks -= 1;
                    _timeSinceDamage = 0;
                    _firstDamage = false;
                }
            }
            else if (_stacks > 0)
            {
                //wait for 1 secs
                if (_timeSinceDamage >= 1)
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

        public void Knockback(Transform source)
        {
            _rgd.AddRelativeForce(source.position.normalized * _knockbackForce, ForceMode.Impulse);
        }
    }
}