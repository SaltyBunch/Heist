using System;
using System.Collections;
using System.Collections.Generic;
using Camera;
using Game;
using Hazard;
using Level;
using Pickup;
using UI;
using UnityEngine;
using Weapon;
using Random = UnityEngine.Random;

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

        [SerializeField] internal bool QuickTimeA;
        [SerializeField] internal bool QuickTimeB;
        [SerializeField] internal bool QuickTimeX;
        [SerializeField] internal bool QuickTimeY;
    }

    [RequireComponent(typeof(Player), typeof(Rigidbody), typeof(AudioSource))]
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private GoldPickup _goldPickup;

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

        [Header("Sounds")] [SerializeField] private AudioClip _footstep;
        [SerializeField] private AudioClip _meleeAttack;
        [SerializeField] private AudioClip _pickupTrap;
        [SerializeField] private AudioClip _pickupWeapon;

        [Space(24)] [SerializeField] private GameObject _reticule;
        [SerializeField] private LayerMask _retLayerMask;
        [SerializeField] private float _retMaxDist;
        [SerializeField] private Rigidbody _rigid;
        [SerializeField] private AudioClip _spotSecurity;
        [SerializeField] private List<AudioClip> _takeDamage;
        [SerializeField] private AudioClip _taunt;
        [SerializeField] private AudioClip _victory;


        public int PlayerNumber;

        public CameraLogic CameraLogic;

        public Player BaseCharacter => _baseCharacter;

        [Header("Animation")] [SerializeField] private Animator _anim;
        private bool _isAnimNotNull;
        [SerializeField] private PlayerModel _playerModel;


        private Tuple<GameObject, string> _interactObject;
        [SerializeField] private LayerMask _interactMask;
        private Collider[] _interactHits = new Collider[5];
        [SerializeField] private MeshRenderer _reticuleMesh;

        private MaterialPropertyBlock _prop;
        private bool _stunParticleExists;
        [SerializeField] private ParticleSystem _stunParticle;

        public bool QTEInteracting;

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

        private void Awake()
        {
            _isAnimNotNull = _anim != null;
            if (_baseCharacter == null)
                _baseCharacter = GetComponent<Player>();
            if (_rigid == null)
                _rigid = GetComponent<Rigidbody>();
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            _isReticuleNotNull = _reticule != null;
            if (_isReticuleNotNull)
            {
                GameManager.SetLayerOnAll(_reticule, GameManager.GetPlayerMask(PlayerNumber, false));
            }
            //if (_reticule != null) _reticule = Instantiate(_reticule, this.transform);


            ///////////////////////////////////////////////////
            /////////    Set Layer For Cameras    /////////////
            ///////////////////////////////////////////////////

            gameObject.layer = GameManager.GetPlayerMask(PlayerNumber, false);

            GameManager.SetLayerOnAll(gameObject, gameObject.layer);

            GameManager.SetLayerOnAll(_reticule, 5);

            _stunParticleExists = _stunParticle != null;

            BaseCharacter.HealthChanged += BaseCharacterOnHealthChanged;
            BaseCharacter.Inventory.SelectionChanged += InventoryOnSelectionChanged;
            BaseCharacter.CharacterStunned += BaseCharacterOnCharacterStunned;
            BaseCharacter.Inventory.GoldAmountChanged += InventoryOnGoldAmountChanged;
        }

        private void InventoryOnGoldAmountChanged(object sender, GoldAmountChangedEvent e)
        {
            BaseCharacter.PlayerUiManager.GetGold(e.Amount);
        }

        private void Start()
        {
            _playerModel.SetMaterial(GameManager.GameManagerRef.Skins[PlayerNumber]);
            _playerModel.SetPlayer(PlayerNumber);
            StartCoroutine(Blink(5));

            if (_isReticuleNotNull)
            {
                _prop = new MaterialPropertyBlock();
                _reticuleMesh.GetPropertyBlock(_prop);
                _prop.SetColor("_Color", PlayerModel.Colors[PlayerNumber]);
                _reticuleMesh.SetPropertyBlock(_prop);
            }
        }

        private void BaseCharacterOnCharacterStunned(object sender, EventArgs e)
        {
            if (_loseGold != null) LevelManager.LevelManagerRef.PlayVoiceLine(_loseGold);
            //todo drop gold
            var goldAmount = Mathf.Min(_baseCharacter.Inventory.GoldAmount, 1000);
            _baseCharacter.Inventory.GoldAmount -= goldAmount;
            if (goldAmount > 0)
            {
                var goldPickup = Instantiate(_goldPickup, transform.position + transform.up, Quaternion.identity);
                goldPickup.SetDisableFor(PlayerNumber);
                StartCoroutine(UnlockGold(5, goldPickup));
            }

            OnMoveCancel?.Invoke(this, new EventArgs());
            if (_isAnimNotNull)
                _anim.SetTrigger("Stunned");

            if (_stunParticleExists)
                _stunParticle.Play();
        }
        IEnumerator UnlockGold(float wait, GoldPickup goldPickUp)
        {
            yield return new WaitForSeconds(wait);
            goldPickUp.SetEnableAll();
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
                e.Item.gameObject.transform.localPosition = Vector3.zero;
                e.Item.gameObject.transform.localRotation = Quaternion.identity;
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

            _playerUiManager.SetControlHint(e.Type);
            _playerUiManager.SetItem(e.Item);
            _playerUiManager.UpdateAmmo(e.Count);
        }

        private void BaseCharacterOnHealthChanged(object sender, HealthChangedEventArgs e)
        {
            if (e.AmountChanged < 0 && _takeDamage != null)
            {
                _playerModel.TakeDamage();
                LevelManager.LevelManagerRef.PlayVoiceLine(_takeDamage[Random.Range(0, _takeDamage.Count)]);
            }

            _playerUiManager.SetHealth(e.Health);
        }

        private void Pause()
        {
            Debug.Log("Pause Request by Player " + (PlayerNumber + 1));
        }

        private void FixedUpdate()
        {
            _playerModel.hidey = Control.MoveVector.magnitude;

            if (Math.Abs(Control.FaceVector.magnitude) < 0.001f)
            {
                var control = Control;
                control.FaceVector = Control.MoveVector;
                Control = control;
                if (_playerModel.FaceState != PlayerModel.FacesState.Idle) _playerModel.SetIdle();
            }

            if (_baseCharacter.Stunned)
            {
                var control = Control;
                control.MoveVector = Vector3.zero;
                control.FaceVector = Vector3.zero;
                Control = control;
                if (_playerModel.FaceState != PlayerModel.FacesState.Stun) _playerModel.SetStunned();
            }

            if (Math.Abs(Control.MoveVector.magnitude) > 0.01f)
                OnMoveCancel?.Invoke(this, new EventArgs());

            ///Direction based on FaceVector
            var facing = Vector3.RotateTowards(transform.forward, Control.FaceVector, 1,
                0.0f);
            transform.rotation = Quaternion.LookRotation(facing);
            /// Movement based on MoveVector


            _rigid.AddForce(Control.MoveVector, ForceMode.VelocityChange);
            if ((_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).magnitude >
                (!_baseCharacter.InElectric ?  _baseCharacter.Stats.Speed : _baseCharacter.Stats.Speed - 2))
            {
                var velocity = Vector3.Lerp(_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward,
                    (_rigid.velocity.x * Vector3.right + _rigid.velocity.z * Vector3.forward).normalized *
                    (!_baseCharacter.InElectric ? _baseCharacter.Stats.Speed : _baseCharacter.Stats.Speed - 2), .5f);
                velocity += _rigid.velocity.y * Vector3.up;
                _rigid.velocity = velocity;
            }

            /// Reticule


            if (_isAnimNotNull)
            {
                _anim.SetFloat("Speed", Control.MoveVector.magnitude); //todo use a blendtree for better anims
                _anim.SetBool("Shoot", _baseCharacter.Inventory.SelectedItem is StunGun && !_baseCharacter.Stunned);
                _anim.SetBool("Shotgun", _baseCharacter.Inventory.SelectedItem is Shotgun && !_baseCharacter.Stunned);
            }

            var size = Physics.OverlapSphereNonAlloc(transform.position + transform.up, _interactDistance,
                _interactHits, _interactMask);
            if (size == 0)
            {
                _interactObject = new Tuple<GameObject, string>(null, "");
                if (!(_baseCharacter.OverWeaponPickup || _baseCharacter.OverTrapPickup))
                {
                    _playerUiManager.SetOpen("None", false);
                }
            }

            for (var i = 0; i < size; i++)
            {
                var hit = _interactHits[i];

                if (hit.gameObject == _interactObject.Item1) break;

                if (hit.transform.CompareTag("Door"))
                {
                    _interactObject = new Tuple<GameObject, string>(hit.gameObject, "Door");
                    _playerUiManager.SetOpen("Door", hit.gameObject.GetComponentInParent<Door>().IsOpen);
                    break;
                }
                else if (hit.transform.CompareTag("HazardDisabler"))
                {
                    _interactObject = new Tuple<GameObject, string>(hit.gameObject, "HazardDisabler");
                    _playerUiManager.SetOpen("HazardDisabler", false);
                    break;
                }
                else if (hit.transform.CompareTag("GoldPile"))
                {
                    _interactObject = new Tuple<GameObject, string>(hit.gameObject, "GoldPile");
                    _playerUiManager.SetOpen("GoldPile", false);
                    break;
                }
                else if (hit.transform.CompareTag("MiniVault"))
                {
                    _interactObject = new Tuple<GameObject, string>(hit.gameObject, "MiniVault");
                    _playerUiManager.SetOpen("MiniVault", false);
                    break;
                }
                else if (hit.transform.CompareTag("Vault"))
                {
                    _interactObject = new Tuple<GameObject, string>(hit.gameObject, "Vault");
                    _playerUiManager.SetOpen("Vault", false);
                    break;
                }
                else
                {
                    _interactObject = new Tuple<GameObject, string>(null, "");
                    if (!(_baseCharacter.OverWeaponPickup || _baseCharacter.OverTrapPickup))
                    {
                        _playerUiManager.SetOpen("None", false);
                    }
                }
            }

            if (_stunParticleExists)
            {
                _stunParticle.gameObject.transform.position = _playerModel.Head.position + Vector3.up*0.7f;
                if (BaseCharacter.Stunned && !_stunParticle.isPlaying)
                {
                    _stunParticle.Play();
                }
                else if (!BaseCharacter.Stunned && _stunParticle.isPlaying)
                {
                    _stunParticle.Stop();
                }
            }

            if (_isReticuleNotNull)
            {
                if (_control.FaceVector.magnitude > 0)
                {
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
            if (_baseCharacter.Stunned || QTEInteracting) return;
            if (_isAnimNotNull)
            {
                if (_baseCharacter.Inventory.SelectedItem is StunGun)
                {
                    _anim.SetTrigger("Shot");
                }
                else if (_baseCharacter.Inventory.SelectedItem is Baton)
                {
                    _anim.SetTrigger("Baton");
                }
                else if (_baseCharacter.Inventory.SelectedItem is Hazard.Hazard)
                {
                    _anim.SetTrigger("Place");
                }
            }

            if (BaseCharacter.Inventory.Use() && Random.Range(0, 8) == 0)
            {
                LevelManager.LevelManagerRef.PlayVoiceLine(_taunt);
            }
        }

        private void Interact()
        {
            if (_baseCharacter.Stunned || QTEInteracting) return;
            if (_baseCharacter.OverWeaponPickup || _baseCharacter.OverTrapPickup)
            {
                _anim.SetTrigger("PickUp");
                _baseCharacter.PickupPickup();
            }
            else
            {
                switch (_interactObject.Item2)
                {
                    case "Door":
                        var door = _interactObject.Item1.GetComponentInParent<Door>();
                        door.Open(this);
                        break;
                    case "HazardDisabler":
                        var hazardDisabler = _interactObject.Item1.GetComponent<HazardDisabler>();
                        hazardDisabler.DisableHazard(this);
                        break;
                    case "GoldPile":
                        var gold = _interactObject.Item1.GetComponent<GoldPile>();
                        gold.StartChanneling(this);
                        break;
                    case "MiniVault":
                        var miniVault = _interactObject.Item1.GetComponent<MiniVault>();
                        miniVault.StartChanneling(this);
                        break;
                    case "Vault":
                        var vault = _interactObject.Item1.GetComponent<Vault>();
                        vault.UseKey(BaseCharacter.Inventory.keys, BaseCharacter);
                        break;
                }

                _interactObject = new Tuple<GameObject, string>(null, "None");
            }
        }

        private void PushAttack()
        {
            if (_baseCharacter.Stunned || QTEInteracting) return;
            _anim.SetTrigger("Baton");
            var objects = Physics.OverlapSphere(transform.position, 2);
            foreach (var o in objects)
            {
                if (o.CompareTag("Player") || o.CompareTag("Drone"))
                {
                    var character = o.GetComponentInParent<Character>();
                    if (character.gameObject.layer != this.gameObject.layer)
                    {
                        character.Knockback(transform);
                        _audioSource.clip = _meleeAttack;
                        _audioSource.Play();
                        break;
                    }
                }
            }
        }

        private void Dash()
        {
            if (_dashCooldown && !(_baseCharacter.Stunned || QTEInteracting))
            {
                if (_isAnimNotNull && Control.MoveVector.magnitude > 0.01f)
                {
                    _anim.SetTrigger("Dash");
                    LevelManager.LevelManagerRef.Notify(transform.position, NotifyType.Dash);
                    StartCoroutine(DashCooldown());
                }
            }
        }

        private IEnumerator DashCooldown()
        {
            _dashCooldown = false;
            _rigid.AddForce(_dashForce * 3f * Control.MoveVector, ForceMode.VelocityChange);
            yield return new WaitForSeconds(_dashTimer);
            _dashCooldown = true;
        }

        private IEnumerator Blink(int timer)
        {
            do
            {
                yield return new WaitForSeconds(timer);
                if (_playerModel.FaceState == PlayerModel.FacesState.Idle)
                {
                    _playerModel.SetBlink();
                    yield return new WaitForSeconds(0.5f);
                    _playerModel.SetIdle();
                }
            } while (true);
        }

        private void OnDestroy()
        {
            BaseCharacter.HealthChanged -= BaseCharacterOnHealthChanged;
            BaseCharacter.Inventory.SelectionChanged -= InventoryOnSelectionChanged;
            BaseCharacter.CharacterStunned -= BaseCharacterOnCharacterStunned;
            BaseCharacter.Inventory.GoldAmountChanged -= InventoryOnGoldAmountChanged;
        }

        public void PickupGold()
        {
            _audioSource.clip = _collectGold;
            _audioSource.Play();
        }

        public void PickupWeapon()
        {
            if (_pickupWeapon != null && Random.Range(0, 2) == 0)
                LevelManager.LevelManagerRef.PlayVoiceLine(_pickupWeapon);
        }

        public void PickupTrap()
        {
            if (_pickupTrap != null && Random.Range(0, 2) == 0)
                LevelManager.LevelManagerRef.PlayVoiceLine(_pickupTrap);
        }
    }
}