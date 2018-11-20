using System.Linq;
using Character;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager LevelManagerRef;

        [SerializeField] private PlayerGameObject _playerGo;

        private PlayerGameObject[] _players;

        [SerializeField] private GameObject[] _spawnpoints;

        private void Awake()
        {
            if (LevelManagerRef == null || LevelManagerRef == this) LevelManagerRef = this;
            else Destroy(this.gameObject);

            if (_spawnpoints == null) _spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        }

        private void Start()
        {
            var controllers = Input.GetJoystickNames();

            string output = "";

            for (var i = 0; i < controllers.Length; i++)
            {
                var controller = controllers[i];
                output += controller + " assigned to Player " + (i + 1) + "\n";
            }

            Debug.Log(output);

            InitGame(4);
        }

        public static int CalculateScore(Player player)
        {
            var score = player.Inventory.GoldAmount * 100;
            //todo add more sources of score
            return score;
        }

        public void InitGame(int numPlayers)
        {
            int displays = 1;
            if (GameManager.UseMultiScreen)
            {
                // Number of displays
#if UNITY_EDITOR || UNITY_EDITOR_64
                displays = 4;
#else
                    displays = Display.displays.Length;
                #endif

                for (var i = 0; i < Display.displays.Length; i++)
                {
                    var display = Display.displays[i];
                    if (!display.active)
                        display.Activate();
                }
            }

            #region Level Setup

            //todo place hazards

            #endregion

            #region Drone Setup

            //todo drone setup

            #endregion

            #region Player Setup

            _players = new PlayerGameObject[numPlayers];

            ShuffleSpawns();

            var playersPerDisplay = numPlayers / displays;

            var targetDisplay = 0;

            for (var i = 0; i < numPlayers; i++)
            {
                if (i % playersPerDisplay == 0 && i != 0)
                    targetDisplay++;
                _players[i] = Instantiate(_playerGo);
                //todo set appropriate player models

                //put player on spawnpoint
                _players[i].Player.transform.position = _spawnpoints[i].transform.position;

                //set screen region
                switch (playersPerDisplay)
                {
                    case 1:
                        _players[i].Camera.Camera.rect = new Rect
                        {
                            x = 0,
                            y = 0,
                            width = 1,
                            height = 1
                        };
                        break;
                    case 2:
                        _players[i].Camera.Camera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = 0,
                            width = 0.5f,
                            height = 1
                        };
                        break;
                    case 3:
                    case 4:
                        _players[i].Camera.Camera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = Mathf.Abs(1 - i / 2) * 0.5f,
                            width = 0.5f,
                            height = 0.5f
                        };
                        break;
                }

                _players[i].Camera.Camera.targetDisplay = targetDisplay;
                //assign player number
                _players[i].PlayerControl.PlayerNumber = i;
            }

            #endregion
        }

        private void ShuffleSpawns()
        {
            for (int i = 0; i < _spawnpoints.Length; i++)
            {
                var newLoc = Random.Range(0, _spawnpoints.Length - 1);

                var temp = _spawnpoints[i];
                _spawnpoints[i] = _spawnpoints[newLoc];
                _spawnpoints[newLoc] = temp;
            }
        }
    }
}