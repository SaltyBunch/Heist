using System.Collections;
using UnityEngine;

namespace Character
{
    public class Character : MonoBehaviour
    {
        private int _stacks;

        public int Stacks //todo add stack decreasing 20 sec for first, 5 thereafter
        {
            get { return _stacks; }
            set
            {
                if (!(_stun || _stunCooldown || _damageCooldown))
                {
                    _stacks = value;
                    StartCoroutine(DamageCooldown());
                    if (_stacks >= 4)
                    {
                        StartCoroutine(Stun());
                    }
                }
            }
        }

        private IEnumerator DamageCooldown()
        {
            _damageCooldown = true;
            yield return new WaitForSeconds(2);
            _damageCooldown = false;
        }

        private bool _stun = false;
        private bool _stunCooldown = false;
        private bool _damageCooldown = false;


        private IEnumerator Stun()
        {
            _stun = true;
            yield return new WaitForSeconds(5);
            _stun = false;
            _stunCooldown = true;
            yield return new WaitForSeconds(3);
            _stunCooldown = false;
        }
    }
}