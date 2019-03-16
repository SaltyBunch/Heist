using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _character;

        [SerializeField] private int _playerNumber;

        private static List<Color> _colors = new List<Color>()
        {
            Color.red, Color.blue, Color.green, Color.cyan
        };

        private MaterialPropertyBlock _prop;

        public void SetMaterial(Material mat)
        {
            var temp = _character.sharedMaterials;
            temp[0] = mat;
            _character.sharedMaterials = temp;
        }

        private void Update()
        {
            if (_playerNumber >= 0)
            {
                _character.GetPropertyBlock(_prop);
                _prop.SetColor("_OccludedColor", _colors[_playerNumber]);
                _character.SetPropertyBlock(_prop);
            }
        }

        public void SetPlayer(int playerNumber)
        {
            _prop = new MaterialPropertyBlock();
            _playerNumber = playerNumber;
        }
    }
}