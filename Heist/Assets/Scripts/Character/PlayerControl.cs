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
        public Player BaseCharacter => _baseCharacter;
        [SerializeField] private Control _control;
        private bool _dashCooldown = true;

        [SerializeField] [Range(10, 30)] private float _dashForce;
        [SerializeField] private float _dashTimer = 0.5f;

        [Header("Sounds")] [SerializeField] private AudioClip _meleeAttack;

        [Header("Voice Lines")] [SerializeField]
        private AudioClip _enterBank;

        [SerializeField] private AudioClip _exitBank;
        [SerializeField] private AudioClip _pickupWeapon;
        [SerializeField] private AudioClip _pickupTrap;
        [SerializeField] private AudioClip _spotSecurity;
        [SerializeField] private AudioClip _takeDamage;
        [SerializeField] private AudioClip _collectGold;
        [SerializeField] private AudioClip _loseGold;
        [SerializeField] private AudioClip _taunt;
        [SerializeField] private AudioClip _joke;
        [SerializeField] private AudioClip _victory;
        [SerializeField] private AudioClip _defeat;

        [Space(24)] [SerializeField] private GameObject _reticule;
        [SerializeField] private LayerMask _retLayerMask;
        [SerializeField] private float _retMaxDist;
        [SerializeField] private Rigidbody _rigid;

        public Rewired.Player Player;

        public int PlayerNumber;
        [SerializeField] private float _interactDistance;

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

            BaseCharacter.HealthChanged += BaseCharacterOnHealthChanged;
            BaseCharacter.Inventory.SelectionChanged += InventoryOnSelectionChanged;
        }

        private void InventoryOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UI.UIManager.UiManagerRef.UpdateWeapon(e.Item, PlayerNumber);
            UI.UIManager.UiManagerRef.UpdateAmmo(e.Count, PlayerNumber);
        }

        private void BaseCharacterOnHealthChanged(object sender, HealthChangedEventArgs e)
        {
            UI.UIManager.UiManagerRef.UpdateHealth(e.Health, PlayerNumber);
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
                    _reticule.transform.localPosition = Vector3.up;
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
            RaycastHit hit;

            if (_baseCharacter.OverWeaponPickup || _baseCharacter.OverTrapPickup) _baseCharacter.PickupPickup();
            else if (Physics.Raycast(transform.position, _control.FaceVector, out hit, _interactDistance))
            {
                if (hit.transform.CompareTag("Door"))
                {
                }
            }
        }

        private void PushAttack()
        {
            Debug.Log("Push Attack by Player " + (PlayerNumber + 1));
        }

        private void Dash()
        {
            if (_dashCooldown)
            {
                LevelManager.LevelManagerRef.Notify(transform.position, NotifyType.Dash);
                StartCoroutine(DashCooldown());
                _rigid.AddForce(Control.MoveVector * _dashForce * 2.5f, ForceMode.VelocityChange);
            }
        }

        private IEnumerator DashCooldown()
        {
            _dashCooldown = false;
            _rigid.AddForce(Control.MoveVector * _dashForce, ForceMode.VelocityChange);
            yield return new WaitForSeconds(_dashTimer / 8);
            _rigid.AddForce(Control.MoveVector * _dashForce, ForceMode.VelocityChange);
            yield return new WaitForSeconds(_dashTimer / 8);
            _rigid.AddForce(Control.MoveVector * _dashForce, ForceMode.VelocityChange);
            yield return new WaitForSeconds((_dashTimer / 4) * 3);
            _dashCooldown = true;
        }

        private void OnDestroy()
        {
            BaseCharacter.HealthChanged -= BaseCharacterOnHealthChanged;
        }
    }
}