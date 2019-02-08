using UnityEngine;

namespace Weapon
{
    public class StunGun : Weapon
    {
        [SerializeField] private Projectile _projectile;

        public void Attack()
        {
            var proj = Instantiate(_projectile, transform.position, transform.rotation);
            proj.Shoot();
        }
    }
}