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
        [SerializeField] private CameraMoveLogic _camera;

        private enum ControlType
        {
            Mouse,
            Controller,
            Rewired
        }

        private enum ControlBinding
        {
            Default,
            Modified,
        }

        [SerializeField] private ControlBinding _controlBinding = ControlBinding.Modified;

        [SerializeField] private ControlType _controlType;
        private Rewired.Player _player;

        private void Awake()
        {
            if (_playerControl == null)
                _playerControl = GetComponent<PlayerControl>();
        }

        private void Start()
        {
            _player = ReInput.players.GetPlayer(_playerControl.PlayerNumber);
        }

        private void Update()
        {
            var playerControlControl = _playerControl.Control;

            switch (_controlType)
            {
                case ControlType.Mouse:
                    RaycastHit hit;
                    if (Physics.Raycast(_camera.Camera.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        var dif = hit.point - transform.position;
                        playerControlControl.FaceVector = (dif.x * Vector3.right + dif.z * Vector3.forward).normalized;
                    }

                    playerControlControl.Dash = Input.GetKey("space");
                    playerControlControl.Interact = Input.GetKey("f");

                    playerControlControl.WeaponAttack = Input.GetMouseButton(0);
                    playerControlControl.PushAttack = Input.GetMouseButton(1);

                    break;
                case ControlType.Controller:
                    //todo finish controller controls
                    playerControlControl.MoveVector =
                        Input.GetAxis("Joystick Left " + _playerControl.PlayerNumber + " Vertical") *
                        _camera.transform.forward * -1 +
                        Input.GetAxis("Joystick Left " + _playerControl.PlayerNumber + " Horizontal") *
                        _camera.transform.right;

                    playerControlControl.FaceVector =
                        Input.GetAxis("Joystick Right " + _playerControl.PlayerNumber + " Vertical") *
                        _camera.transform.forward * -1 +
                        Input.GetAxis("Joystick Right " + _playerControl.PlayerNumber + " Horizontal") *
                        _camera.transform.right;

                    switch (_controlBinding)
                    {
                        case ControlBinding.Default: //based off of GDD Bindings
                            playerControlControl.Dash =
                                Input.GetKey("joystick " + (_playerControl.PlayerNumber + 1) + " button 0"); //A button
                            playerControlControl.Interact =
                                Input.GetKey("joystick " + (_playerControl.PlayerNumber + 1) + " button 2"); //X Button

                            playerControlControl.PushAttack =
                                Input.GetKey("joystick " + (_playerControl.PlayerNumber + 1) + " button 1"); //B button
                            playerControlControl.WeaponAttack =
                                Input.GetAxis("Joystick Right Trigger " + _playerControl.PlayerNumber) > 0.5f;
                            break;
                        case ControlBinding.Modified: //Personal Preference
                            playerControlControl.Dash =
                                Input.GetKey("joystick " + (_playerControl.PlayerNumber + 1) + " button 5"); //RB button
                            playerControlControl.Interact =
                                Input.GetKey("joystick " + (_playerControl.PlayerNumber + 1) + " button 4"); //LB button

                            playerControlControl.PushAttack =
                                Input.GetAxis("Joystick Left Trigger " + _playerControl.PlayerNumber) > 0.5f;
                            playerControlControl.WeaponAttack =
                                Input.GetAxis("Joystick Right Trigger " + _playerControl.PlayerNumber) > 0.5f;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                    playerControlControl.Pause =
                        Input.GetKey("joystick " + (_playerControl.PlayerNumber + 1) + " button 7"); //Start button


                    break;
                case ControlType.Rewired:
                    playerControlControl.MoveVector = _player.GetAxis("Move Vertical") * Vector3.forward +
                                                      _player.GetAxis("Move Horizontal") * Vector3.right;

                    playerControlControl.FaceVector = _player.GetAxis("Look Vertical") * Vector3.forward +
                                                      _player.GetAxis("Look Horizontal") * Vector3.right;

                    playerControlControl.Dash = _player.GetButton("Dash");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.DrawRay(transform.position + Vector3.up, transform.forward * 2, Color.red);


            _playerControl.Control = playerControlControl;
        }
    }
}