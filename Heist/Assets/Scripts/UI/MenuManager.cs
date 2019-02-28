using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        public static MenuManager menuManager;

        private GameObject current;
        [SerializeField] private EventSystem ES;
        private readonly string GameScene = "SampleScene";
        private int inGame;
        [SerializeField] private LoadingScreen loadingScreen;
        [SerializeField] private MasterMixer masterMixer;
        [SerializeField] private Menu menu;
        [SerializeField] private List<string> playernames;

        [SerializeField] private List<GameObject> playersImages;
        [SerializeField] private List<PlayerSelect> selction;
        [SerializeField] private VictoryScreen victoryScreen;

        [SerializeField] private UnityEngine.Camera _mainCamera;

        private void Start()
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
            if (selction.TrueForAll(s => s.ready || !s.gameObject.activeSelf) && inGame == 0) RestartGame();


            if (Input.GetKeyDown(KeyCode.N) && Input.GetKeyDown(KeyCode.B) && inGame == 2) ToMain();
            if (Input.GetKeyDown(KeyCode.N) && Input.GetKeyDown(KeyCode.B) && inGame == 1) EndGame();
        }

        public void ToMain()
        {
            StartCoroutine(UNLoadScene());
            if (current) current.SetActive(false);
            menu.gameObject.SetActive(true);
            ES.SetSelectedGameObject(menu.mainMenuButton);
            current = menu.gameObject;
            inGame = 0;
        }

        private IEnumerator UNLoadScene()
        {
            yield return new WaitForEndOfFrame();
            var asyncOperation = SceneManager.UnloadSceneAsync(GameScene);
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

        public void RestartGame()
        {
            LoadingScreen();
            StartCoroutine(LoadScene());
            inGame = 1;

            //set playerimage and player names
        }

        private IEnumerator LoadScene()
        {
            yield return new WaitForEndOfFrame();
            var asyncOperation = SceneManager.LoadSceneAsync(GameScene, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                if (asyncOperation.progress >= 0.9f)
                {
                    InGameUI();
                    asyncOperation.allowSceneActivation = true;
                    _mainCamera.gameObject.SetActive(false);
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
            current = null;
        }
    }
}