using UnityEngine;

namespace Weapon
{
    public class Baton : Weapon
    {
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