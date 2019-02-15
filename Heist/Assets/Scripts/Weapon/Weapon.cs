using System;
using Game;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(AudioSource))]
    public class Weapon : Item, IEquatable<Weapon>
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _fireSound;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private AudioClip _outOfAmmoSound;

        [SerializeField] private AudioClip _pickupSound;
        [SerializeField] [Range(5, 30)] private int _pushForce;


        public int Ammo;

        public bool Equals(Weapon other)
        {
            return this is Baton && other is Baton || this is StunGun && other is StunGun;
        }

        public void Bind(GameObject player)
        {
            throw new NotImplementedException();
        }

        public void Attack()
        {
        }
    }
}