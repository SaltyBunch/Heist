using System;
using Character;
using Game;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;

namespace Camera
{
    public class CameraLogic : MonoBehaviour
    {
        [SerializeField] [Range(0, 30)] private float _bounds = 3;
        [SerializeField] private LayerMask _layerMask;

        //[SerializeField] private GameObject _mask;

        [SerializeField] private Vector3 _offset;
        [SerializeField] private PlayerControl _playerControl;

        [SerializeField] private bool _seesPlayer;
        [SerializeField] [Range(0, 1)] private float _smoothing = 0.2f;

        [SerializeField] public UnityEngine.Camera MainCamera;
        [SerializeField] public UnityEngine.Camera UICamera;

        private void Start()
        {
            MainCamera.transform.localPosition = _offset;
            MainCamera.transform.LookAt(_playerControl.transform.position);
            MainCamera.cullingMask = MainCamera.cullingMask;
        }

        private void FixedUpdate()
        {
            var trackedPosition = _playerControl.transform.position + _playerControl.Control.MoveVector * _bounds +
                                  _playerControl.Control.FaceVector * _bounds / 2;

            transform.position = Vector3.Lerp(transform.position, trackedPosition, _smoothing);

            // transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward,
            //    _playerControl.Control.FaceVector, 1, 0.0f));
        }
    }
}