using Character;
using Game;
using UnityEngine;

namespace Camera
{
    public class CameraLogic : MonoBehaviour
    {
        [SerializeField] private PlayerControl _playerControl;

        [SerializeField] public UnityEngine.Camera Camera;

        [SerializeField, Range(0, 30)] private float _bounds = 3;
        [SerializeField, Range(0, 1)] private float _smoothing = 0.2f;

        [SerializeField] private float _offset = 10;

        [SerializeField] private GameObject _mask;
        [SerializeField] private LayerMask _layerMask;

        private void Start()
        {
            Camera.transform.localPosition = Vector3.back * _offset + Vector3.up * _offset;

            Camera.cullingMask += GameManager.GetPlayerMask(_playerControl.PlayerNumber, true);
        }

        void Update()
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.transform.position, (_playerControl.transform.position- Camera.transform.position).normalized,
                out hit, 100, _layerMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    _mask.GetComponent<MaskActivate>().StopShow(); 
                }
                else _mask.GetComponent<MaskActivate>().StartShow();
            }
            Debug.DrawRay(Camera.transform.position, (_playerControl.transform.position - Camera.transform.position).normalized, Color.red);
        }

        private void FixedUpdate()
        {
            Vector3 trackedPosition = _playerControl.transform.position +
                                      _playerControl.Control.MoveVector * _bounds +
                                      _playerControl.Control.FaceVector * _bounds / 2;

            transform.position = Vector3.Lerp(transform.position, trackedPosition, _smoothing);
        }
    }
}