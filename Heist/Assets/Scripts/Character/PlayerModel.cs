using UnityEngine;

namespace Character
{
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private Material _characterMaterial;
        [SerializeField] private SkinnedMeshRenderer _character;

        private void Start()
        {
            _characterMaterial = _character.sharedMaterial;
        }

        public void SetColor(Color color)
        {
            _characterMaterial.color = color;
        }
    }
}