using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody), typeof(Character))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Character _baseCharacter;

        [SerializeField] private Rigidbody _rigid;

        internal Vector2 MoveVector;
        
        
        private void Awake()
        {
            if (_baseCharacter == null)
                _baseCharacter = GetComponent<Character>();

            if (_rigid == null)
                _rigid = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            //todo movement from movevector
        }
    }
}