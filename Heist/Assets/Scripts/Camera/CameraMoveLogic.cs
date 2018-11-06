using Character;
using UnityEngine;

namespace Camera
{
    public class CameraMoveLogic : MonoBehaviour
    {
        [SerializeField] private PlayerMovement _playerMovement;

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
            Vector3 trackedPosition = _playerMovement.transform.position +
                                      _playerMovement.MoveVector * _bounds +
                                      _playerMovement.FaceVector * _bounds;

            //make tracked position center of screen

            transform.position = Vector3.Lerp(transform.position, trackedPosition, _smoothing);
        }
    }
}