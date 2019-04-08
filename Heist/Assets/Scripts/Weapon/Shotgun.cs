using UnityEngine;

namespace Weapon
{
    public class Shotgun : Weapon
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] public Vector3 Barrel;


        public void Attack()
        {
            _audioSource.clip = _fireSound;
            _audioSource.Play();
            for (int i = -1; i <= 1; i++)
            {
                var proj = Instantiate(_projectile, transform.TransformPoint(Barrel) + transform.right * i * 0.15f,
                    Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, Vector3.up), Vector3.up) *
                    Quaternion.Euler(0, 5 * i, 0));
                proj.Shoot();
            }

            Ammo--;
        }
    }
}