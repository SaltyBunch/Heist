using UnityEngine;

namespace Weapon
{
    public class StunGun : Weapon
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] public Vector3 Barrel;


        public new void Attack()
        {
            var proj = Instantiate(_projectile, transform.TransformPoint(Barrel), transform.rotation);
            proj.Shoot();
        }
    }
}