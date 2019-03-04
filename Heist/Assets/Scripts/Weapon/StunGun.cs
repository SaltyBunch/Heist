using Game;
using UnityEngine;

namespace Weapon
{
    public class StunGun : Weapon
    {
        [SerializeField] private Projectile _projectile;
        [SerializeField] public Vector3 Barrel;


        public new void Attack(int playerNum)
        {
            _audioSource.clip = _fireSound;
            _audioSource.Play();
            var proj = Instantiate(_projectile, transform.TransformPoint(Barrel), transform.rotation);
            GameManager.SetLayerOnAll(proj.gameObject, GameManager.GetPlayerMask(playerNum, false));
            proj.Shoot();
            Ammo--;
        }
    }
}