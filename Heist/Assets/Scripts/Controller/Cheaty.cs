using Game;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class Cheaty : MonoBehaviour
    {
        [SerializeField] private Transform _vaultLocation;
        [SerializeField] private Transform _lobbyLocation;

        //todo cheaty buttons


        private void Update()
        {
            foreach (var player in ReInput.players.Players)
            {
                if (player.controllers.hasKeyboard)
                {
                    var keyboard = player.controllers.Keyboard;
                    if (keyboard.GetKeyDown(keyCode: KeyCode.T))
                    {
                        EndGame();
                    }
                }
            }
        }

        private void EndGame()
        {
            if (SceneManager.GetActiveScene().name == GameManager.GameManagerRef.SceneNames.GameScene)
            {
                LevelManager.LevelManagerRef.AllPlayersLeft(false);
            }
        }
    }
}