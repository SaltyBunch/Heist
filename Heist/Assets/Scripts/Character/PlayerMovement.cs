using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(Rigidbody), typeof(Player))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private Player _baseCharacter;

        [SerializeField] private Rigidbody _rigid;

        internal Vector3 MoveVector;
        internal Vector3 FaceVector;


        private void Awake()
        {
            if (_baseCharacter == null)
                _baseCharacter = GetComponent<Player>();
            if (_rigid == null)
                _rigid = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            ///Direction based on FaceVector
            var facing = Vector3.RotateTowards(transform.forward, FaceVector, 1,
                0.0f);
            transform.rotation = Quaternion.LookRotation(facing);
            /// Movement based on MoveVector
            _rigid.AddForce(MoveVector, ForceMode.VelocityChange);
            if ((_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).magnitude >
                _baseCharacter.Speed)
            {
                var velocity = (_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).normalized *
                               _baseCharacter.Speed;
                velocity += _rigid.velocity.y * Vector3.up;
                _rigid.velocity = velocity;
            }
        }
    }
}