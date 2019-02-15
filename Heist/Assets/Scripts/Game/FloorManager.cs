using UnityEngine;

namespace Game
{
    public class FloorManager : MonoBehaviour
    {
        [SerializeField] private GameObject _basement;
        [SerializeField] private GameObject _mainFloor;

        private void Awake()
        {
        }
    }
}