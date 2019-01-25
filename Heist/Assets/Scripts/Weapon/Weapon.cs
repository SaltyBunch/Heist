using UnityEngine;

namespace Weapon
{
    [RequireComponent(typeof(AudioSource))]
    public class Weapon : MonoBehaviour
    {
        [SerializeField, Range(5, 30)]private int _pushForce;

        [SerializeField] private AudioClip _pickupSound;
        [SerializeField] private AudioClip _fireSound;
        [SerializeField] private AudioClip _hitSound;
        [SerializeField] private AudioClip _outOfAmmoSound;

        [SerializeField] private AudioSource _audioSource;


        public void Bind(GameObject player)
        {
            throw new System.NotImplementedException();
        }
    }
}