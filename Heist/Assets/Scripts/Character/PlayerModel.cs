using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Character
{
    public class PlayerModel : MonoBehaviour
    {
        public enum FacesState
        {
            Blink,
            Defeat,
            Idle,
            Smile,
            Speak,
            Stun
        }

        [SerializeField] private List<SkinnedMeshRenderer> _characterSkinnedMeshRenderers;

        [SerializeField] private MeshRenderer _face;

        [SerializeField] private List<Texture2D> _faces;
        [SerializeField] private FacesState _facesState = FacesState.Idle;

        [SerializeField] private int _playerNumber = -1;

        public float hidey = 0;
        private float _modelAlpha = 4;

        private static List<Color> _colors = new List<Color>()
        {
            Color.red, Color.blue, Color.green, Color.cyan
        };

        private MaterialPropertyBlock _prop;

        private void Awake()
        {
            _prop = new MaterialPropertyBlock();
        }

        public void SetMaterial(Material mat)
        {
            foreach (var character in _characterSkinnedMeshRenderers)
            {
                var temp = character.sharedMaterials;
                temp[0] = mat;
                character.sharedMaterials = temp;
            }
        }

        private void Update()
        {
            if (_playerNumber != -1)
            {
                if (hidey > 0) _modelAlpha = 4;
                else _modelAlpha -= Time.deltaTime;
                var v = _colors[_playerNumber];
                v.a = Mathf.Clamp(_modelAlpha, 0, 0.6f);
                UpdateColor(v);
                UpdateFace(v);
            }
            else
            {
                UpdateColor(Color.clear);
                UpdateFace(Color.clear);
            }
        }

        private void UpdateFace(Color color)
        {
            _face.GetPropertyBlock(_prop);
            _prop.SetColor("_OccludedColor", color);
            _prop.SetTexture("_MainTex", _faces[(int) _facesState]);
            _face.SetPropertyBlock(_prop);
        }

        private void UpdateColor(Color color)
        {
            foreach (var character in _characterSkinnedMeshRenderers)
            {
                character.GetPropertyBlock(_prop);
                _prop.SetColor("_OccludedColor", color);
                character.SetPropertyBlock(_prop);
            }
        }

        public void SetPlayer(int playerNumber)
        {
            _playerNumber = playerNumber;
        }
    }
}