using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public enum Characters
    {
        King,
        Jailbird,
        Shadow,
        Raccoon
    } 
    public class GameManager : MonoBehaviour
    {
        public static readonly Dictionary<Characters, Stats> CharacterStats = new Dictionary<Characters, Stats>
        {
            {
                Characters.King, new Stats
                {
                    Health = 5,
                    Speed = 2,
                    Dexterity = 2,
                }
            },
            {
                Characters.Jailbird, new Stats
                {
                    Health = 4,
                    Speed = 5,
                    Dexterity = 3,
                }
            },
            {
                Characters.Shadow, new Stats
                {
                    Health = 3,
                    Speed = 4,
                    Dexterity = 5,
                }
            },
            {
                Characters.Raccoon, new Stats
                {
                    Health = 4,
                    Speed = 4,
                    Dexterity = 4,
                }
            },
        };

        public static GameManager GameManagerRef;

        [SerializeField]
        private LoadingScreen loadingScreen;
        [SerializeField]
        private PauseMenu pauseMenu;
        private bool canPause = true;
        private string currentLevelName;

        public static bool UseMultiScreen = true;

        //store the players character choice here
        public static Characters[] PlayerChoice =
            {Characters.Raccoon, Characters.Jailbird, Characters.Shadow, Characters.King};
         
        private void Awake()
        {
            if (GameManagerRef == null || GameManagerRef == this) GameManagerRef = this;
            else Destroy(this.gameObject);

            DontDestroyOnLoad(this.gameObject);
        }

        public void LoadScene(string levelName)
        {
            StartCoroutine("LoadLevel", levelName);
        }

        private IEnumerator LoadLevel(string levelName)
        {

            loadingScreen.gameObject.SetActive(true);

            yield return new WaitForSeconds(.25f);

            if ((!string.IsNullOrEmpty(currentLevelName)))
            {
                //yield return SoundManager.Instance.StartCoroutine("UnLoadLevel"); Sound stuff


                AsyncOperation asyncUnload = SceneManager.UnloadSceneAsync(currentLevelName);

                while (!asyncUnload.isDone)
                {
                    yield return null;
                }
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);
            while (!asyncLoad.isDone)
            {
                yield return null;
                //loadingScreen.UpdateSlider(asyncLoad.progress); Loading slider
            }

            yield return new WaitForSeconds(.75f);

            SceneManager.SetActiveScene(SceneManager.GetSceneByName(levelName));
            //SoundManager.LevelLoadComplete();

            currentLevelName = levelName;


            loadingScreen.gameObject.SetActive(false);

        }
    }
}