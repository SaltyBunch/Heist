using System;
using Camera;
using Character;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(PlayerMovement), typeof(PlayerControl))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;
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
            if (_playerMovement == null)
                _playerMovement = GetComponent<PlayerMovement>();
            if (_playerControl == null)
                _playerControl = GetComponent<PlayerControl>();
        }

        private void Update()
        {
            switch (_controlType)
            {
                case ControlType.Mouse:
                    RaycastHit hit;
                    if (Physics.Raycast(_camera.Camera.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        var dif = hit.point - transform.position;

                        _playerMovement.FaceVector = (dif.x * Vector3.right + dif.z * Vector3.forward).normalized;
                    }

                    var playerControlControl = _playerControl.Control;


                    playerControlControl.Dash = Input.GetButtonDown("Space");
                    playerControlControl.Interact = Input.GetButtonDown("F");


                    _playerControl.Control = playerControlControl;
                    break;
                case ControlType.Controller:
                    //if ()
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.DrawRay(transform.position + Vector3.up, transform.forward * 2, Color.red);

            _playerMovement.MoveVector = Input.GetAxis("Vertical") * _camera.transform.forward +
                                         Input.GetAxis("Horizontal") * _camera.transform.right;
        }
    }
}