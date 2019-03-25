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

    public delegate void StunnedEventHandler(object sender, EventArgs e);

    public class HealthChangedEventArgs
    {
        public int Health;
        public int AmountChanged;
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

        public bool Stunned
        {
            get { return _stun; }
            set
            {
                if (value != _stun && value == true) CharacterStunned?.Invoke(this, EventArgs.Empty);
                _stun = value;
            }
        }

        private bool _stunCooldown;

        private float _timeSinceDamage;
        [SerializeField] public Stats Stats;

        internal int timesStunned;

        public int Stacks
        {
            get => _stacks;
            set
            {
                var changed = _stacks - value;
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

                HealthChanged?.Invoke(this,
                    new HealthChangedEventArgs {Health = Stats.Health - _stacks, AmountChanged = changed});
            }
        }

        public event HealthUpdatedEventHandler HealthChanged;

        public event StunnedEventHandler CharacterStunned;

        public void Start()
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
            Stunned = true;
            timesStunned += 1;
            yield return new WaitForSeconds(5);
            Stunned = false;
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