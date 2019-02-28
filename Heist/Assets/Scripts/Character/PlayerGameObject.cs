using Camera;
using UI;
using UnityEngine;

namespace Character
{
    public class PlayerGameObject : MonoBehaviour
    {
        [SerializeField] public CameraLogic Camera;
        [SerializeField] public Player Player;
        [SerializeField] public PlayerControl PlayerControl;
        [SerializeField] public PlayerUIManager PlayerUiManager;
    }
}