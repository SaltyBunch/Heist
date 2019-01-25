using UnityEngine;

namespace Level
{
    public class MiniVault : MonoBehaviour
    {
        

        [SerializeField] private bool _canHoldKey;
        [SerializeField] private Game.KeyType _key;
        [SerializeField] private int _goldAmount;

        private void Reset()
        {
            gameObject.tag = "MiniVault";
        }
    }
}