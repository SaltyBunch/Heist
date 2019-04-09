using System;
using Game;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(AudioSource))]
    public class Weapon : Item, IEquatable<Weapon>
    {
        [SerializeField] internal AudioSource _audioSource;
        [SerializeField] internal AudioClip _fireSound;
        [SerializeField] internal AudioClip _hitSound;
        [SerializeField] internal AudioClip _outOfAmmoSound;

        [SerializeField] internal AudioClip _pickupSound;


        public int Ammo;

        public bool Equals(Weapon other)
        {
            return this is Baton && other is Baton || this is StunGun && other is StunGun ||
                   this is Shotgun && other is Shotgun;
        }

        public void Attack(int playerNum)
        {
        }

    }
}