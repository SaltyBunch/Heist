using UnityEngine;

namespace Level
{
    public class Interactable : MonoBehaviour
    {
        public enum Type
        {
            None,
            Door,
            GoldPile,
            MiniVault,
            Vault
        }
    }
}