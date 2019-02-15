using System.ComponentModel;
using JetBrains.Annotations;
using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float _shootForce;

        [SerializeField] private Rigidbody _rgd;

        public void Shoot(Vector3? position = null, Quaternion? rotation = null)
        {
            if (position == null)
                position = transform.position;
            if (rotation == null)
                rotation = transform.rotation;

            transform.position = position.Value;
            transform.rotation = rotation.Value;

            _rgd.AddForce(_shootForce * transform.forward);
        }

        private void OnCollisionEnter(Collision other)
        {
            var character = other.transform.GetComponentInParent<Character.Character>();
            if (character != null)
            {
                character.Stacks += 1;
                character.Knockback(this.transform);
            }

            Destroy(this.gameObject);
        }
    }
}