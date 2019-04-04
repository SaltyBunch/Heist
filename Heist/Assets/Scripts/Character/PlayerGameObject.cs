using Camera;
using Controller;
using UI;
using UnityEngine;

namespace Character
{
    public class PlayerGameObject : MonoBehaviour
    {
        [SerializeField] public CameraLogic Camera;
        [SerializeField] public Player Player;
        [SerializeField] public PlayerControl PlayerControl;
        [SerializeField] public PlayerController PlayerController;
        [SerializeField] public PlayerUIManager PlayerUiManager;
        [SerializeField] public PlayerFOG fog;
    }
}