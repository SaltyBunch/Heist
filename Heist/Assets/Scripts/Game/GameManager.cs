using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Rewired;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public enum Characters
    {
        King,
        Jailbird,
        Shadow,
        Raccoon,
        Drone
    }

    [Serializable]
    struct Scenes
    {
        [SerializeField] public string MainMenu;
        [SerializeField] public string GameScene;
        [SerializeField] public string VictoryScreen;
        [SerializeField] public string Persistent;
    }

    enum State
    {
        Menu,
        Game,
        Victory
    }

    public struct Score
    {
        public int GoldAmount;
        public int TimesStunned;

        public int PlayerNumber;

        public int PlayerScore =>
            (int) (GoldAmount * GameManager.GoldMultiplier - TimesStunned * GameManager.StunnedMultiplier);
    }

    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Scenes _scenes;

        public static readonly Dictionary<Characters, Stats> CharacterStats = new Dictionary<Characters, Stats>
        {
            {
                Characters.King, new Stats
                {
                    Health = 5,
                    Speed = 2,
                    Dexterity = 2
                }
            },
            {
                Characters.Jailbird, new Stats
                {
                    Health = 4,
                    Speed = 5,
                    Dexterity = 3
                }
            },
            {
                Characters.Shadow, new Stats
                {
                    Health = 3,
                    Speed = 4,
                    Dexterity = 5
                }
            },
            {
                Characters.Raccoon, new Stats
                {
                    Health = 4,
                    Speed = 4,
                    Dexterity = 4
                }
            },
            {
                Characters.Drone, new Stats
                {
                    Health = 3,
                    Speed = 3,
                    Dexterity = -1
                }
            }
        };

        public static GameManager GameManagerRef;

        [SerializeField] private State _state;
        [SerializeField] private LoadingScreen _loadingScreen;

        public bool UseMultiScreen = true;

        //store the players character choice here
        public static Characters[] PlayerChoice =
            {Characters.Raccoon, Characters.Jailbird, Characters.Shadow, Characters.King};

        public static float GoldMultiplier = 10;
        public static float StunnedMultiplier = 100;
        public List<Score> Scores;
        public static int NumPlayers { get; set; }

        public static List<Material> Skins = new List<Material>()
        {
            null, null, null, null
        };


        private void Awake()
        {
            if (GameManagerRef == null || GameManagerRef == this) GameManagerRef = this;
            else Destroy(gameObject);
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartCoroutine(LoadScene(_scenes.MainMenu));
            NumPlayers = ReInput.controllers.joystickCount;

        }

        public static int GetPlayerMask(int playerNumber, bool bitShift)
        {
            return bitShift ? 1 << (28 + playerNumber) : 28 + playerNumber;
        }

        public static void SetLayerOnAll(GameObject obj, int layer)
        {
            foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true))
            {
                trans.gameObject.layer = layer;
            }
        }

        public void EnterGame()
        {
            StartCoroutine(LoadScene(_scenes.GameScene));
            _state = State.Game;

            var activeScene = SceneManager.GetActiveScene().name;
            if (activeScene != _scenes.Persistent)
                StartCoroutine(UnLoadScene(activeScene));
        }

        private IEnumerator LoadScene(string sceneName)
        {
            _loadingScreen.gameObject.SetActive(true);
            yield return new WaitForEndOfFrame();
            var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncOperation.allowSceneActivation = false;
            while (!asyncOperation.isDone)
            {
                _loadingScreen.Progress = asyncOperation.progress;
                if (asyncOperation.progress >= 0.9f)
                {
                    asyncOperation.allowSceneActivation = true;
                }

                yield return null;
            }

            var scene = SceneManager.GetSceneByName(sceneName);
            SceneManager.SetActiveScene(scene);
            SetupScene(sceneName);
            _loadingScreen.gameObject.SetActive(false);
        }

        private void SetupScene(string sceneName)
        {
            if (sceneName == _scenes.GameScene)
            {
                LevelManager.LevelManagerRef.InitGame(NumPlayers);
            }
        }

        public void EndGame()
        {
            StartCoroutine(LoadScene(_scenes.VictoryScreen));
            _state = State.Victory;

            var activeScene = SceneManager.GetActiveScene().name;
            if (activeScene != _scenes.Persistent)
                StartCoroutine(UnLoadScene(activeScene));
        }

        private IEnumerator UnLoadScene(string scene)
        {
            yield return new WaitForEndOfFrame();
            var asyncOperation = SceneManager.UnloadSceneAsync(scene);
            while (!asyncOperation.isDone)
            {
                yield return null;
            }
        }

        public void ToMain()
        {
            StartCoroutine(LoadScene(_scenes.MainMenu));
            _state = State.Game;

            var activeScene = SceneManager.GetActiveScene().name;
            if (activeScene != _scenes.Persistent)
                StartCoroutine(UnLoadScene(activeScene));
        }
    }
}