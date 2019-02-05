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
        [SerializeField] UIManager uiManager;

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
                current = menu.gameObject;
            }
        }

        public void RestartGame()
        {
            LoadingScreen();
            StartCoroutine(LoadScene());

            //set playerimage and player names
        }

        IEnumerator LoadScene()
        {
            yield return null;
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(GameScene, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                if (asyncOperation.progress >= 0.9f)
                {
                    InGameUI();
                    asyncOperation.allowSceneActivation = true;
                }
                yield return null;
            }
        
            
        }

        public void EndGame()
        {
            if (current) current.SetActive(false);
            victoryScreen.Initialize(playernames, playersImages);
            victoryScreen.gameObject.SetActive(true);
            current = victoryScreen.gameObject;
            
            
        }

        public void LoadingScreen()
        {
            if (current) current.SetActive(false);
            loadingScreen.gameObject.SetActive(true);
            current = loadingScreen.gameObject;
            
        }

        public void InGameUI()
        {
            if (current) current.SetActive(false);
            //create UI
            uiManager.gameObject.SetActive(true);
            current = uiManager.gameObject;
        }
    }
}
