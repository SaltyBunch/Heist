using System;
using System.Collections;
using Game;
using UnityEngine;

namespace Character
{
    [Serializable]
    internal struct Control
    {
        [SerializeField] internal bool Dash;
        [SerializeField] internal bool PushAttack;

        [SerializeField] internal bool Interact;
        [SerializeField] internal bool WeaponAttack;

        [SerializeField] internal bool SwitchWeapon;
        [SerializeField] internal bool SwitchTrap;

        [SerializeField] internal Vector3 MoveVector;
        [SerializeField] internal Vector3 FaceVector;

        [SerializeField] internal bool Pause;
    }

    [RequireComponent(typeof(Player), typeof(Rigidbody), typeof(AudioSource))]
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Player _baseCharacter;

        [SerializeField] private Control _control;
        private bool _dashCooldown = true;

        [SerializeField] [Range(10, 30)] private float _dashForce;
        [SerializeField] private float _dashTimer = 0.5f;

        [SerializeField] private AudioClip _meleeAttack;

        [SerializeField] private GameObject _reticule;
        [SerializeField] private LayerMask _retLayerMask;
        [SerializeField] private float _retMaxDist;
        [SerializeField] private Rigidbody _rigid;

        public Rewired.Player Player;

        public int PlayerNumber;

        internal Control Control
        {
            get => _control;
            set
            {
                if (!Equals(value, _control))
                {
                    if (value.Dash && !_control.Dash)
                        Dash();
                    if (value.Interact && !_control.Interact)
                        Interact();
                    if (value.WeaponAttack && !_control.WeaponAttack)
                        WeaponAttack();
                    if (value.PushAttack && !_control.PushAttack)
                        PushAttack();
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


        private void Start()
        {
            if (_baseCharacter == null)
                _baseCharacter = GetComponent<Player>();
            if (_rigid == null)
                _rigid = GetComponent<Rigidbody>();
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            if (GameManager.CharacterStats.TryGetValue(GameManager.PlayerChoice[PlayerNumber],
                out _baseCharacter.Stats))
            {
            }
            else
            {
                Debug.LogError("Unexpected result when assigning stats for player " + (PlayerNumber + 1) +
                               " with character choice " + GameManager.PlayerChoice[PlayerNumber]);
            }

            if (_reticule != null) _reticule.layer = GameManager.GetPlayerMask(PlayerNumber, false);

            //if (_reticule != null) _reticule = Instantiate(_reticule, this.transform);
        }

        private void Pause()
        {
            Debug.Log("Pause Request by Player " + (PlayerNumber + 1));
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
                _baseCharacter.Stats.Speed)
            {
                var velocity = Vector3.Lerp(_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward,
                    (_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).normalized *
                    _baseCharacter.Stats.Speed, .5f);
                velocity += _rigid.velocity.y * Vector3.up;
                _rigid.velocity = velocity;
            }

            /// Reticule
            if (_reticule != null)
            {
                if (_control.FaceVector.magnitude > 0)
                {
                    RaycastHit hitInfo;
                    _reticule.transform.localPosition = Physics.Raycast(transform.position, transform.forward,
                        out hitInfo, _retMaxDist, _retLayerMask)
                        ? transform.InverseTransformPoint(hitInfo.point)
                        : _retMaxDist / 2 * Vector3.forward + Vector3.up;
                }
                else
                {
                    _reticule.transform.position = Vector3.up;
                }
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

        private void WeaponAttack()
        {
            Debug.Log("Weapon Attack by Player " + (PlayerNumber + 1));
        }

        private void Interact()
        {
            if (_baseCharacter.OverWeaponPickup || _baseCharacter.OverTrapPickup) _baseCharacter.PickupPickup();
        }

        private void PushAttack()
        {
            Debug.Log("Push Attack by Player " + (PlayerNumber + 1));
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