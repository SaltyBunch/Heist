using System;
using Camera;
using Character;
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
            Controller
        }

        [SerializeField] private ControlType _controlType;

        private void Awake()
        {
            if (_playerControl == null)
                _playerControl = GetComponent<PlayerControl>();
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

                    playerControlControl.Dash = Input.GetKeyDown("space");
                    playerControlControl.Interact = Input.GetKeyDown("f");

                    playerControlControl.RangeAttack = Input.GetMouseButtonDown(0);
                    playerControlControl.MeleeAttack = Input.GetMouseButtonDown(1);

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

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.DrawRay(transform.position + Vector3.up, transform.forward * 2, Color.red);


            _playerControl.Control = playerControlControl;
        }
    }
}