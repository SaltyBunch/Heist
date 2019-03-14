using Game;
using UnityEngine;

namespace Level
{
    public class MiniVault : MonoBehaviour
    {
        [SerializeField] private bool _canHoldKey;
        [SerializeField] private int _goldAmount;
        [SerializeField] private KeyType _key;

        private void Reset()
        {
            gameObject.tag = "MiniVault";
        }
    }
}