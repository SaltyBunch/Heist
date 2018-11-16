using Character;
using UnityEngine;

namespace Camera
{
    public class CameraMoveLogic : MonoBehaviour
    {
        [SerializeField] private PlayerControl _playerControl;

        [SerializeField] public UnityEngine.Camera Camera;

        [SerializeField, Range(0, 10)] private float _bounds = 3;
        [SerializeField, Range(0, 1)] private float _smoothing = 0.2f;

        [SerializeField] private float _offset = 10;

        private void Start()
        {
            Camera.transform.position = Vector3.back * _offset + Vector3.up * _offset;
        }

        private void FixedUpdate()
        {
            Vector3 trackedPosition = _playerControl.transform.position +
                                      _playerControl.Control.MoveVector * _bounds +
                                      _playerControl.Control.FaceVector * _bounds;

            transform.position = Vector3.Lerp(transform.position, trackedPosition, _smoothing);
        }
    }
}