using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI
{

    public class MenuManager : MonoBehaviour
    {
        [SerializeField] EventSystem ES;

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
        int inGame = 0;

        void Start()
        {
            if (!menuManager)
            {
                menuManager = this;
                DontDestroyOnLoad(this);
                current = menu.gameObject;
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.N) && Input.GetKeyDown(KeyCode.B) && inGame == 2) ToMain();
            if (Input.GetKeyDown(KeyCode.N) && Input.GetKeyDown(KeyCode.B) && inGame == 1) EndGame();
            
        }

        public void ToMain()
        {
            SceneManager.UnloadSceneAsync(GameScene);
            if (current) current.SetActive(false);
            menu.gameObject.SetActive(true);
            ES.SetSelectedGameObject(menu.mainMenuButton);
            current = menu.gameObject;
            inGame = 0;
        }

        public void RestartGame()
        {
            LoadingScreen();
            StartCoroutine(LoadScene());
            inGame = 1;

            //set playerimage and player names
        }

        IEnumerator LoadScene()
        {
            yield return new WaitForEndOfFrame();
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
            //victoryScreen.Initialize(playernames, playersImages);  TODO with proepr inputs
            victoryScreen.gameObject.SetActive(true);
            current = victoryScreen.gameObject;
            inGame = 2;
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
