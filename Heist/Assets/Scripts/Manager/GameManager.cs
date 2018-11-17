using System.Linq;
using Character;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager GameMananger;

        [SerializeField] private PlayerGameObject _playerGo;

        private PlayerGameObject[] _players;

        [SerializeField] private GameObject[] _spawnpoints;

        private void Awake()
        {
            if (GameMananger == null || GameMananger != this) GameMananger = this;
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
                output += controller + " assigned to Player " + (i+1) + "\n";
            }

            Debug.Log(output);

            InitGame(controllers.Length);
        }

        public static int CalculateScore(Player player)
        {
            var score = player.Inventory.GoldAmount * 100;
            //todo add more sources of score
            return score;
        }

        public void InitGame(int numPlayers)
        {
            _players = new PlayerGameObject[numPlayers];

            ShuffleSpawns();

            for (var i = 0; i < numPlayers; i++)
            {
                _players[i] = Instantiate(_playerGo);
                //todo set appropriate player models

                //put player on spawnpoint
                _players[i].Player.transform.position = _spawnpoints[i].transform.position;

                //set screen region
                switch (numPlayers)
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
                            x = 0,
                            y = i % 2 * 0.5f,
                            width = 1,
                            height = 0.5f
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
                //assign player number
                _players[i].PlayerControl.PlayerNumber = i;
            }
        }

        private void ShuffleSpawns()
        {
            for (int i = 0; i < _spawnpoints.Length; i++)
            {
                int newLoc = Random.Range(0, _spawnpoints.Length - 1);

                var temp = _spawnpoints[i];
                _spawnpoints[i] = _spawnpoints[newLoc];
                _spawnpoints[newLoc] = temp;
            }
        }
    }
}