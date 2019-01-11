using System.Collections;
using UnityEngine;

namespace Character
{
    [System.Serializable]
    internal struct Control
    {
        [SerializeField] internal bool Dash;
        [SerializeField] internal bool MeleeAttack;

        [SerializeField] internal bool Interact;
        [SerializeField] internal bool RangeAttack;

        [SerializeField] internal bool SwitchWeapon;
        [SerializeField] internal bool SwitchTrap;

        [SerializeField] internal Vector3 MoveVector;
        [SerializeField] internal Vector3 FaceVector;

        [SerializeField] internal bool Pause;
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

            if (Game.GameManager.CharacterStats.TryGetValue(Game.GameManager.PlayerChoice[PlayerNumber],
                out _baseCharacter.Stats))
            {
            }
            else
                Debug.LogError("Unexpected result when assigning stats for player " + (PlayerNumber + 1) +
                               " with character choice " + Game.GameManager.PlayerChoice[PlayerNumber]);
        }

        [SerializeField] private Control _control;

        [SerializeField, Range(10, 30)] private float _dashForce;
        private bool _dashCooldown = true;
        private float _dashTimer = 0.5f;

        [SerializeField, Range(0, 10), Tooltip("Addition to Speed")] private float _addSpeed = 0;
        [SerializeField, Range(1, 10), Tooltip("Multiplication of Speed")] private float _mulSpeed = 1;

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
                    if (value.Pause && !_control.Pause)
                        Pause();
                }

                _control = value;
            }
        }

        private void Pause()
        {
            Debug.Log("Pause Request by Player " + (PlayerNumber + 1));
        }

        public int PlayerNumber;

        private void FixedUpdate()
        {
            ///Direction based on FaceVector
            var facing = Vector3.RotateTowards(transform.forward, Control.FaceVector, 1,
                0.0f);
            transform.rotation = Quaternion.LookRotation(facing);
            /// Movement based on MoveVector
            _rigid.AddForce(Control.MoveVector, ForceMode.VelocityChange);
            if ((_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).magnitude >
                _baseCharacter.Stats.Speed)
            {
                var velocity = Vector3.Lerp(_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward,
                    (_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).normalized *
                    (_baseCharacter.Stats.Speed + _addSpeed) *_mulSpeed, .5f);
                velocity += _rigid.velocity.y * Vector3.up;
                _rigid.velocity = velocity;
            }
        }

        private void SwitchTrap()
        {
            Debug.Log("Switch Trap by Player " + (PlayerNumber + 1));
        }

        private void SwitchWeapon()
        {
            Debug.Log("Switch Weapon by Player " + (PlayerNumber + 1));
        }

        private void RangeAttack()
        {
            Debug.Log("Range Attack by Player " + (PlayerNumber + 1));
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
            Debug.Log("Melee Attack by Player " + (PlayerNumber + 1));
        }

        private void Dash()
        {
            if (_dashCooldown)
            {
                StartCoroutine(DashCooldown());
                _rigid.AddForce(Control.MoveVector * _dashForce * 2.5f, ForceMode.VelocityChange);
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