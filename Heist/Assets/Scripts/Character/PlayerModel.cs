using UnityEngine;

namespace Character
{
    public class PlayerModel : MonoBehaviour
    {
        [SerializeField] private SkinnedMeshRenderer _character;
        
        public void SetMaterial(Material mat)
        {
            _character.sharedMaterial = mat;
        }
    }
}