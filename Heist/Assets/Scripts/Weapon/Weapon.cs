using UnityEngine;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField, Range(5, 30)]private int _pushForce;

        public void Bind()
        {
            throw new System.NotImplementedException();
        }
    }
}