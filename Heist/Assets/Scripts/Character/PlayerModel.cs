using UnityEngine;

namespace Character
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _character;

        public void SetMaterial(Material mat)
        {
            var temp = _character.sharedMaterials;
            temp[0] = mat;
            _character.sharedMaterials = temp;
        }
    }
}