using UnityEngine;

namespace Weapon
{
    public class Baton : Weapon
    {
        private void OnTriggerEnter(Collider other)
        {
            var character = other.transform.GetComponentInParent<Character.Character>();
            if (character != null)
            {
                character.Stacks += 1;
                character.Knockback(transform);
            }
        }

        public void Use()
        {
            Ammo--;
        }
    }
}