using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private List<SkinnedMeshRenderer> _characterSkinnedMeshRenderers;

        [SerializeField] private int _playerNumber = -1;

        public float hidey = 0;
        private float _modelAlpha = 4;

        private static List<Color> _colors = new List<Color>()
        {
            Color.red, Color.blue, Color.green, Color.cyan
        };

        private MaterialPropertyBlock _prop;

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
                _modelAlpha = Mathf.Clamp(_modelAlpha, 0, 1);
                v.a = _modelAlpha;
                _colors[_playerNumber] = v;
                UpdateColor();
            }
        }

        private void UpdateColor()
        {
            if (_playerNumber >= 0)
            {
                foreach (var character in _characterSkinnedMeshRenderers)
                {
                    character.GetPropertyBlock(_prop);
                    _prop.SetColor("_OccludedColor", _colors[_playerNumber]);
                    character.SetPropertyBlock(_prop);
                }
            }
        }

        public void SetPlayer(int playerNumber)
        {
            _prop = new MaterialPropertyBlock();
            _playerNumber = playerNumber;
            UpdateColor();
        }
    }
}