using System.Collections.Generic;
using UnityEngine;
namespace Character
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private List<SkinnedMeshRenderer> _characterSkinnedMeshRenderers;

        [SerializeField] private int _playerNumber;

        public float hidey = 0;
        private float modelAlpha = 6.0f;

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
            if (hidey > 0) modelAlpha = 6;
            else modelAlpha -= Time.deltaTime;
            var v = _colors[_playerNumber];
            v.a = Mathf.Min(modelAlpha, 1);
            _colors[_playerNumber] = v;
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