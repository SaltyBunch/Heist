using System.Collections;
using UnityEngine;

namespace Character
{
    [System.Serializable]
    internal struct Control
    {
        internal bool Dash;
        internal bool MeleeAttack;

        internal bool Interact;
        internal bool RangeAttack;

        internal bool SwitchWeapon;
        internal bool SwitchTrap;

        internal Vector3 MoveVector;
        internal Vector3 FaceVector;
    }

    [RequireComponent(typeof(Player), typeof(Rigidbody))]
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private Player _baseCharacter;
        [SerializeField] private Rigidbody _rigid;


        private void Start()
        {
            if (_baseCharacter == null)
                _baseCharacter = GetComponent<Player>();
            if (_rigid == null)
                _rigid = GetComponent<Rigidbody>();
        }

        [SerializeField] private Control _control;

        [SerializeField, Range(1, 10)] private float _dashForce;
        private bool _dashCooldown;
        private float _dashTimer = 5;

        internal Control Control
        {
            get { return _control; }
            set
            {
                if (!Equals(value, _control))
                {
                    if (value.Dash && !_control.Dash)
                        Dash();
                    if (value.Interact && !_control.Interact)
                        Interact();
                    if (value.RangeAttack && !_control.RangeAttack)
                        RangeAttack();
                    if (value.MeleeAttack && !_control.MeleeAttack)
                        MeleeAttack();
                    if (value.SwitchWeapon && !_control.SwitchWeapon)
                        SwitchWeapon();
                    if (value.SwitchTrap && !_control.SwitchTrap)
                        SwitchTrap();
                }
                _control = value;
            }
        }

        private void FixedUpdate()
        {
            ///Direction based on FaceVector
            var facing = Vector3.RotateTowards(transform.forward, Control.FaceVector, 1,
                0.0f);
            transform.rotation = Quaternion.LookRotation(facing);
            /// Movement based on MoveVector
            _rigid.AddForce(Control.MoveVector, ForceMode.VelocityChange);
            if ((_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).magnitude >
                _baseCharacter.Speed)
            {
                var velocity = Vector3.Lerp(_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward,
                    (_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).normalized *
                    _baseCharacter.Speed, .2f);
                velocity += _rigid.velocity.y * Vector3.up;
                _rigid.velocity = velocity;
            }
        }

        private void SwitchTrap()
        {
            throw new System.NotImplementedException();
        }

        private void SwitchWeapon()
        {
            throw new System.NotImplementedException();
        }

        private void RangeAttack()
        {
            throw new System.NotImplementedException();
        }

        private void Interact()
        {
            if (_baseCharacter.OverWeaponPickup || _baseCharacter.OverTrapPickup)
            {
                _baseCharacter.PickupPickup();
            }
        }

        private void MeleeAttack()
        {
            throw new System.NotImplementedException();
        }

        private void Dash()
        {
            if (_dashCooldown)
            {
                StartCoroutine(DashCooldown());
                _rigid.AddForce(Control.FaceVector * _dashForce, ForceMode.VelocityChange);
            }
        }

        private IEnumerator DashCooldown()
        {
            _dashCooldown = false;
            yield return new WaitForSeconds(_dashTimer);
            _dashCooldown = true;
        }
    }
}