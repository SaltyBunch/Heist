using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{

    public class MenuManager : MonoBehaviour
    {
        public static MenuManager menuManager;
        [SerializeField] Menu menu;
        [SerializeField] MasterMixer masterMixer;
        [SerializeField] VictoryScreen victoryScreen;
        [SerializeField] LoadingScreen loadingScreen;

        [SerializeField] List<GameObject> playersImages;
        [SerializeField] List<string> playernames;

        private GameObject current;
        private string GameScene = "SampleScene";

        void Start()
        {
            if (!menuManager)
            {
                menuManager = this;
                DontDestroyOnLoad(this);
            }
        }

        public void RestartGame()
        {
            SceneManager.LoadScene(GameScene);
            //set playerimage and player names
        }

        public void EndGame()
        {
            if (current) current.SetActive(false);
            current = victoryScreen.gameObject;
            victoryScreen.Initialize(playernames, playersImages);
            victoryScreen.gameObject.SetActive(true);
            
        }

        public void LoadingScreen()
        {
            if (current) current.SetActive(false);
            current = loadingScreen.gameObject;
            loadingScreen.gameObject.SetActive(true);
        }
    }
}
