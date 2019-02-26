using Camera;
using Character;
using Rewired;
using UnityEngine;
using Player = Rewired.Player;

namespace Controller
{
    [RequireComponent(typeof(PlayerControl))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private CameraLogic _camera;
        [SerializeField] private PlayerControl _playerControl;

        public Player Player;

        private void Awake()
        {
            if (_playerControl == null)
                _playerControl = GetComponent<PlayerControl>();
        }

        private void Start()
        {
            Player = _playerControl.Player;
            if (Player == null) Player = ReInput.players.GetPlayer(_playerControl.PlayerNumber - 1);
        }

        private void Update()
        {
            var playerControlControl = _playerControl.Control;


            playerControlControl.MoveVector = Player.GetAxis("Move Vertical") * Vector3.forward +
                                              Player.GetAxis("Move Horizontal") * Vector3.right;

            playerControlControl.FaceVector = Player.GetAxis("Look Vertical") * Vector3.forward +
                                              Player.GetAxis("Look Horizontal") * Vector3.right;

            playerControlControl.Dash = Player.GetButton("Dash");
            playerControlControl.Interact = Player.GetButton("Interact");

            playerControlControl.PushAttack = Player.GetButton("Push Attack");
            playerControlControl.WeaponAttack = Player.GetButton("Weapon Attack");

            playerControlControl.Pause = Player.GetButton("Pause");

            playerControlControl.SwitchPos = Player.GetButton("InventorySwitchPositive");
            playerControlControl.SwitchNeg = Player.GetButton("InventorySwitchNegative");
            //Debug.DrawRay(transform.position + Vector3.up, transform.forward * 2, Color.red);


            _playerControl.Control = playerControlControl;
        }
    }
}