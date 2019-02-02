using System;
using System.Collections;
using System.Threading;
using Audio;
using Character;
using Rewired;
using UnityEngine;
using Player = Character.Player;
using Random = UnityEngine.Random;

namespace Game
{
    [RequireComponent(typeof(AudioSource), typeof(FloorManager))]
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager LevelManagerRef;
        [SerializeField] private int _goldMultiplier = 10;
        [SerializeField] private int _stunMultiplier = 100;

        [SerializeField] private PlayerGameObject _playerGo;

        private PlayerGameObject[] _players;

        [SerializeField] private GameObject[] _spawnpoints;

        private AudioSource[] _audioSource;

        [SerializeField] private AudioClip _backgroundMusicInfiltration;
        [SerializeField] private AudioClip _backgroundMusicGathering;
        [SerializeField] private AudioClip _backgroundMusicLockdown;

        [SerializeField] private float _vaultTimer;

        private float _time;
        private int _currentAudioSource = 0;
        public static float Time => LevelManagerRef._time;

        [SerializeField] private FloorManager _floorManager;
        public FloorManager FloorManager => _floorManager;
        
        private void Awake()
        {
            if (LevelManagerRef == null) LevelManagerRef = this;
            else Destroy(gameObject);
            if (_spawnpoints == null) _spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            if (_audioSource == null) _audioSource = GetComponents<AudioSource>();

            _time = 0;
        }

        private void Start()
        {
            //var players = Rewired.ReInput.players.playerCount;
            var players = ReInput.controllers.joystickCount;
            InitGame(players);
            _audioSource[_currentAudioSource].clip = _backgroundMusicInfiltration;
            _audioSource[_currentAudioSource].Play();

            StartCoroutine(LevelTimer());
        }

        private IEnumerator LevelTimer()
        {
            yield return new WaitForSeconds(0.5f);
            _time += 0.5f;
        }

        public int CalculateScore(Player player)
        {
            var score = player.Inventory.GoldAmount * _goldMultiplier;
            score -= player.timesStunned * _stunMultiplier;
            return score;
        }

        public void InitGame(int numPlayers)
        {
            var displays = 1;
            if (GameManager.UseMultiScreen)
            {
                // Number of displays
#if UNITY_EDITOR || UNITY_EDITOR_64
                //displays = 4;
#else
                    displays = Display.displays.Length;
#endif
            }

            #region Display Setup

            var playersPerDisplay = numPlayers / displays;
            var targetDisplay = 0;
            //find number of players on each screen
            var playersOnDisplay = new int[displays];
            int tempNumPlayers = numPlayers, index = 0;
            while (tempNumPlayers > 0)
            {
                playersOnDisplay[index] += 1;
                tempNumPlayers -= 1;
                index = (index + 1) % displays;
            }

            for (var j = 0; j < playersOnDisplay.Length; j++)
                if (playersOnDisplay[j] == 0)
                {
                    displays = j + 1;
                    break;
                }

            for (var i = 0; i < Display.displays.Length; i++)
            {
                var display = Display.displays[i];
                if (!display.active && i < displays)
                    display.Activate();
            }

            #endregion

            #region Level Setup

            //todo place hazards

            #endregion

            #region Drone Setup

            //todo drone setup

            #endregion

            #region Player Setup

            _players = new PlayerGameObject[numPlayers];

            ShuffleSpawns();


            for (var i = 0; i < numPlayers; i++)
            {
                if (i != 0 && i % playersPerDisplay == 0)
                    targetDisplay++;
                _players[i] = Instantiate(_playerGo);
                //todo set appropriate player models

                //put player on spawnpoint
                _players[i].transform.position = _spawnpoints[i].transform.position;

                //TODO don't split screen when players are alone on the screen
                //set screen region
                switch (playersOnDisplay[targetDisplay])
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

                _players[i].PlayerControl.Player = ReInput.players.GetPlayer(i);
            }

            #endregion
        }

        private void ShuffleSpawns()
        {
            for (var i = 0; i < _spawnpoints.Length; i++)
            {
                var newLoc = Random.Range(0, _spawnpoints.Length - 1);

                var temp = _spawnpoints[i];
                _spawnpoints[i] = _spawnpoints[newLoc];
                _spawnpoints[newLoc] = temp;
            }
        }

        public IEnumerator OpenVault(Action callVault)
        {
            if (_audioSource.Length > 1)
                StartCoroutine(AudioHelper.CrossFade(_audioSource[_currentAudioSource],
                    _audioSource[(_currentAudioSource + 1) % _audioSource.Length], _backgroundMusicLockdown, 5));
            _currentAudioSource = (_currentAudioSource + 1) % _audioSource.Length;
            yield return new WaitForSeconds(_vaultTimer);
            callVault();
        }
    }
}