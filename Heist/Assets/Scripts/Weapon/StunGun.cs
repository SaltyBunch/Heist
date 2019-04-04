using Game;
using UnityEngine;

namespace Weapon
{
    public class StunGun : Weapon
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] public Vector3 Barrel;


        public new void Attack()
        {
            _audioSource.clip = _fireSound;
            _audioSource.Play();
            var proj = Instantiate(_projectile, transform.TransformPoint(Barrel),
                Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up));
            proj.Shoot();
            Ammo--;
        }
    }
}