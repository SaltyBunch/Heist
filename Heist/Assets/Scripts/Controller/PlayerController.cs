using System;
using Camera;
using Character;
using Rewired;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(PlayerControl))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerControl _playerControl;
        [SerializeField] private CameraLogic _camera;

        public Rewired.Player Player;

        private void Awake()
        {
            if (_playerControl == null)
                _playerControl = GetComponent<PlayerControl>();
        }

        private void Start()
        {
            Player = _playerControl.Player;
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


            //Debug.DrawRay(transform.position + Vector3.up, transform.forward * 2, Color.red);


            _playerControl.Control = playerControlControl;
        }
    }
}