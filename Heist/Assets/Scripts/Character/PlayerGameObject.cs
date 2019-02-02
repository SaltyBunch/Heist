using Camera;
using UnityEngine;

namespace Character
{
    public class PlayerGameObject : MonoBehaviour
    {
        [SerializeField] public Player Player;
        [SerializeField] public PlayerControl PlayerControl;
        [SerializeField] public CameraLogic Camera;
    }
}