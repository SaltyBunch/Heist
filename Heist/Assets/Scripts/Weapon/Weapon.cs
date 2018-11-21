using UnityEngine;

namespace Weapon
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField, Range(5, 30)]private int _pushForce;
    }
}