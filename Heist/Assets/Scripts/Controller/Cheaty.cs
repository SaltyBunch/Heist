using System.Collections.Generic;
using Game;
using Level;
using Rewired;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class Cheaty : MonoBehaviour
    {
        private Vector3 _vaultLocation = new Vector3(-196.42f, 1.06f, -124.53f);
        private Vector3 _lobbyLocation = new Vector3(-3.4f, 1.06f, -122.3f);

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
                    if (keyboard.GetKeyDown(keyCode: KeyCode.P))
                    {
                        GoToMain();
                    }
                    if (keyboard.GetKeyDown(keyCode: KeyCode.K))
                    {
                        OpenVault();
                    }

                    if (keyboard.GetKeyDown(keyCode: KeyCode.V))
                    {
                        TeleportPlayers(_vaultLocation);
                    }
                    if (keyboard.GetKeyDown(keyCode: KeyCode.L))
                    {
                        TeleportPlayers(_lobbyLocation);
                    }
                }
            }
        }

        private void GoToMain()
        {
            GameManager.GameManagerRef.ToMain();
        }

        private void TeleportPlayers(Vector3 teleportLocation)
        {
            foreach (var player in LevelManager.LevelManagerRef.Players)
            {
                player.PlayerControl.transform.position = teleportLocation;
                player.PlayerControl.CameraLogic.transform.position = teleportLocation;
            }
        }

        private void OpenVault()
        {
            if (SceneManager.GetActiveScene().name == GameManager.GameManagerRef.SceneNames.GameScene)
            {
                var vault = (Vault)FindObjectOfType(typeof(Vault));
                vault.UseKey(new Dictionary<KeyType, bool>()
                {
                    {KeyType.RedKey, true},
                    {KeyType.YellowKey, true}
                }, null);
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