using System;
using Character;
using UnityEngine;

namespace Controller
{
    [RequireComponent(typeof(PlayerMovement))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;

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
        }

        private void Update()
        {
            switch (_controlType)
            {
                case ControlType.Mouse:
                    RaycastHit hit;
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                    {
                        var dif = hit.point - transform.position;

                        _playerMovement.FaceVector = (dif.x * Vector3.right + dif.z * Vector3.forward).normalized;
                    }
                    break;
                case ControlType.Controller:
                    //if ()
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.DrawRay(transform.position + Vector3.up, transform.forward * 2, Color.red);

            _playerMovement.MoveVector = Input.GetAxis("Vertical") * Vector3.forward +
                                         Input.GetAxis("Horizontal") * Vector3.right;
        }
    }
}