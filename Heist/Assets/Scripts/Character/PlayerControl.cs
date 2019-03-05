using System;
using System.Collections;
using Game;
using Level;
using Rewired.Data.Mapping;
using UI;
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

        [SerializeField] internal bool SwitchPos;
        [SerializeField] internal bool SwitchNeg;

        [SerializeField] internal Vector3 MoveVector;
        [SerializeField] internal Vector3 FaceVector;

        [SerializeField] internal bool Pause;
    }

    [RequireComponent(typeof(Player), typeof(Rigidbody), typeof(AudioSource))]
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private Player _baseCharacter;
        [SerializeField] private PlayerUIManager _playerUiManager;
        [SerializeField] private AudioClip _collectGold;
        [SerializeField] private Control _control;
        private bool _dashCooldown = true;

        [SerializeField] [Range(10, 30)] private float _dashForce;
        [SerializeField] private float _dashTimer = 0.5f;

        [SerializeField] private Transform _hand;
        [SerializeField] private float _interactDistance;
        private bool _isReticuleNotNull;

        [Header("Voice Lines")] [SerializeField]
        private AudioClip _enterBank;


        [SerializeField] private AudioClip _defeat;
        [SerializeField] private AudioClip _joke;
        [SerializeField] private AudioClip _loseGold;
        [SerializeField] private AudioClip _exitBank;

        [Header("Sounds")] [SerializeField] private AudioClip _meleeAttack;
        [SerializeField] private AudioClip _pickupTrap;
        [SerializeField] private AudioClip _pickupWeapon;

        [Space(24)] [SerializeField] private GameObject _reticule;
        [SerializeField] private LayerMask _retLayerMask;
        [SerializeField] private float _retMaxDist;
        [SerializeField] private Rigidbody _rigid;
        [SerializeField] private AudioClip _spotSecurity;
        [SerializeField] private AudioClip _takeDamage;
        [SerializeField] private AudioClip _taunt;
        [SerializeField] private AudioClip _victory;

        public Rewired.Player Player;

        public int PlayerNumber;
        public Floor Floor = Floor.MainFloor;
        public Player BaseCharacter => _baseCharacter;

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
                    if (value.SwitchPos && !_control.SwitchPos)
                        SwitchPos();
                    if (value.SwitchNeg && !_control.SwitchNeg)
                        SwitchNeg();
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

            if (_reticule != null) GameManager.SetLayerOnAll(_reticule, GameManager.GetPlayerMask(PlayerNumber, false));
            //if (_reticule != null) _reticule = Instantiate(_reticule, this.transform);

            _isReticuleNotNull = _reticule != null;

            ///////////////////////////////////////////////////
            /////////    Set Layer For Cameras    /////////////
            ///////////////////////////////////////////////////

            gameObject.layer = GameManager.GetPlayerMask(PlayerNumber, false);

            GameManager.SetLayerOnAll(gameObject, gameObject.layer);


            BaseCharacter.HealthChanged += BaseCharacterOnHealthChanged;
            BaseCharacter.Inventory.SelectionChanged += InventoryOnSelectionChanged;
        }

        private void InventoryOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if weapon, parent to hand and enable
            if (e.Type == Item.Type.Weapon)
            {
                for (var i = 0; i < _hand.transform.childCount; i++)
                {
                    var child = _hand.transform.GetChild(i);
                    child.transform.SetParent(gameObject.transform, false);
                    child.gameObject.SetActive(false);
                }

                e.Item.gameObject.transform.SetParent(_hand, false);
                e.Item.gameObject.transform.position = _hand.position;
                e.Item.gameObject.transform.rotation = _hand.rotation;
                e.Item.gameObject.SetActive(true);
            }
            else
            {
                for (var i = 0; i < _hand.transform.childCount; i++)
                {
                    var child = _hand.transform.GetChild(i);
                    child.transform.SetParent(gameObject.transform, false);
                    child.gameObject.SetActive(false);
                }
            }


            _playerUiManager.SetItem(e.Item);
            _playerUiManager.UpdateAmmo(e.Count);
        }

        private void BaseCharacterOnHealthChanged(object sender, HealthChangedEventArgs e)
        {
            _playerUiManager.SetHealth(e.Health);
        }

        private void Pause()
        {
            Debug.Log("Pause Request by Player " + (PlayerNumber + 1));
        }

        private void FixedUpdate()
        {
            if (Math.Abs(Control.FaceVector.magnitude) < 0.001f)
            {
                var control = Control;
                control.FaceVector = Control.MoveVector;
                Control = control;
            }

            if (_baseCharacter.Stunned)
            {
                var control = Control;
                control.MoveVector = Vector3.zero;
                control.FaceVector = Vector3.zero;
                Control = control;
            }

            if (Math.Abs(Control.MoveVector.magnitude) > 0.01f || Control.WeaponAttack)
                OnMoveCancel?.Invoke(this, new EventArgs());

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
            if (_isReticuleNotNull)
            {
                if (_control.FaceVector.magnitude > 0)
                {
                    RaycastHit hitInfo;
                    if (Physics.Raycast(transform.position, _control.FaceVector, out hitInfo, _retMaxDist,
                        _retLayerMask))
                        _reticule.transform.localPosition = transform.InverseTransformPoint(hitInfo.point);
                    else
                        _reticule.transform.localPosition = _retMaxDist / 2 * Vector3.forward + Vector3.up;


                    _reticule.SetActive(true);
                }
                else
                {
                    _reticule.transform.localPosition = Vector3.up;
                    _reticule.SetActive(false);
                }
            }
        }

        public event EventHandler OnMoveCancel;

        private void SwitchNeg()
        {
            BaseCharacter.Inventory.SelectedIndex--;
        }

        private void SwitchPos()
        {
            BaseCharacter.Inventory.SelectedIndex++;
        }

        private void WeaponAttack()
        {
            if (_baseCharacter.Stunned) return;
            BaseCharacter.Inventory.Use();
            Debug.Log("Weapon Attack by Player " + (PlayerNumber + 1));
        }

        private void Interact()
        {
            if (_baseCharacter.Stunned) return;
            if (_baseCharacter.OverWeaponPickup || _baseCharacter.OverTrapPickup)
            {
                _baseCharacter.PickupPickup();
            }
            else
            {
                var hits = Physics.OverlapSphere(transform.position, _interactDistance);
                foreach (var hit in hits)
                {
                    if (hit.transform.CompareTag("Door"))
                    {
                        var door = hit.transform.GetComponent<Door>();
                        door.Open(this);
                    }
                    else if (hit.transform.CompareTag("GoldPile"))
                    {
                        var gold = hit.transform.GetComponent<GoldPile>();
                        gold.StartChanneling(this);
                    }
                    else if (hit.transform.CompareTag("MiniVault"))
                    {
                        //todo
                    }
                    else if (hit.transform.CompareTag("Vault"))
                    {
                        var vault = hit.transform.GetComponent<Vault>();
                        vault.UseKey(BaseCharacter.Inventory.keys);
                    }
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
            }
        }

        private IEnumerator DashCooldown()
        {
            _dashCooldown = false;
            _rigid.AddForce(Control.MoveVector * _dashForce * 2.5f, ForceMode.VelocityChange);
            yield return new WaitForSeconds(_dashTimer);
            _dashCooldown = true;
        }

        private void OnDestroy()
        {
            BaseCharacter.HealthChanged -= BaseCharacterOnHealthChanged;
        }
    }
}