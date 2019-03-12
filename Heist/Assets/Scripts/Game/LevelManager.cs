using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Character;
using Level;
using Rewired;
using UI;
using UnityEngine;
using Player = Character.Player;
using Random = UnityEngine.Random;

namespace Game
{
    public enum KeyType
    {
        BlueKey,
        RedKey,
        YellowKey
    }

    public enum NotifyType
    {
        TripTrap,
        Dash,
        Footstep
    }

    public class NotifyEventArgs : EventArgs
    {
        public Vector3 Position { get; set; }
        public NotifyType NotifyType { get; set; }
    }

    public class NotifyMessageArgs : EventArgs
    {
        public string Message;
    }

    [RequireComponent(typeof(AudioSource))]
    public class LevelManager : MonoBehaviour
    {
        public delegate void NotifyEventHandler(object sender, NotifyEventArgs e);


        public delegate void NotifyMessageHandler(object sender, NotifyMessageArgs e);


        public static LevelManager LevelManagerRef;

        private AudioSource[] _audioSource;
        [SerializeField] private AudioClip _backgroundMusicGathering;

        [SerializeField] private AudioClip _backgroundMusicInfiltration;
        [SerializeField] private AudioClip _backgroundMusicLockdown;
        private int _currentAudioSource;

        [SerializeField] private int _goldMultiplier = 10;

        [SerializeField] private List<PlayerGameObject> _playerGo;

        private PlayerGameObject[] _players;
        public PlayerGameObject[] Players => _players;

        [SerializeField] private GameObject[] _spawnpoints;
        [SerializeField] private int _stunMultiplier = 100;

        [SerializeField] public GameObject FOG;

        private float _time;

        private float _timeSinceVaultOpened;

        private float TimeSinceVaultOpened
        {
            get { return _timeSinceVaultOpened; }
            set
            {
                _timeSinceVaultOpened = value;
                if (_timeSinceVaultOpened > _endGameAtTime) AllPlayersLeft(false);
            }
        }

        [SerializeField] private float _endGameAtTime;

        [SerializeField] private float _vaultTimer;

        [SerializeField] private Collider _gameEndArea;
        private bool[] _playerLeaving;

        public bool[] PlayerLeaving
        {
            get { return _playerLeaving; }
            set
            {
                _playerLeaving = value;
                if (_playerLeaving.Count(x => x == true) == _players.Length)
                {
                    AllPlayersLeft(true);
                }
            }
        }

        private void AllPlayersLeft(bool b)
        {
            CalculateScore();
            //todo go to score screen
        }

        public Dictionary<NotifyType, float> NotificationRamge = new Dictionary<NotifyType, float>
        {
            {NotifyType.Dash, 10}, {NotifyType.Footstep, 5}, {NotifyType.TripTrap, 100}
        };

        private bool _vaultOpen;

        public static float Time => LevelManagerRef._time;

        public LayerMask EnvironmentLayer;

        public event NotifyEventHandler Notifty;

        [SerializeField] private GameObject[] playerModels;
        private bool _doorOpen;

        private void Awake()
        {
            if (LevelManagerRef == null) LevelManagerRef = this;

            _spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            if (_audioSource == null) _audioSource = GetComponents<AudioSource>();

            _time = 0;
        }

        private void Start()
        {
            //var players = Rewired.ReInput.players.playerCount;
            var players = GameManager.NumPlayers;
            //InitGame(4);
            InitGame(players);

            StartCoroutine(LevelTimer());
        }

        private IEnumerator LevelTimer()
        {
            yield return new WaitForSeconds(0.5f);
            _time += 0.5f;
            if (_vaultOpen) TimeSinceVaultOpened += 0.5f;
        }

        public void CalculateScore()
        {
            GameManager.GameManagerRef.Scores = new List<Score>();
            foreach (var playerGO in _players)
            {
                var player = playerGO.Player;

                GameManager.GameManagerRef.Scores.Add(
                    new Score()
                    {
                        GoldAmount = player.Inventory.GoldAmount,
                        TimesStunned = player.timesStunned,
                        PlayerNumber = playerGO.PlayerControl.PlayerNumber,
                    });
            }
        }

        public void InitGame(int numPlayers)
        {
            var displays = 1;
            if (GameManager.GameManagerRef.UseMultiScreen)
            {
                // Number of displays
#if UNITY_EDITOR || UNITY_EDITOR_64
                displays = 2;
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
                _players[i] = Instantiate(_playerGo[(int)GameManager.PlayerChoice[i]]);
                //todo set appropriate player models


                //put player on spawnpoint
                _players[i].transform.position = _spawnpoints[i].transform.position;

                //set screen region
                switch (playersOnDisplay[targetDisplay])
                {
                    case 1:
                        _players[i].Camera.MainCamera.rect = new Rect
                        {
                            x = 0,
                            y = 0,
                            width = 1,
                            height = 1
                        };
                        break;
                    case 2:
                        _players[i].Camera.MainCamera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = 0,
                            width = 0.5f,
                            height = 1
                        };
                        break;
                    case 3:
                    case 4:
                        _players[i].Camera.MainCamera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = Mathf.Abs(1 - i / 2) * 0.5f,
                            width = 0.5f,
                            height = 0.5f
                        };
                        break;
                }


                _players[i].Camera.UICamera.rect = _players[i].Camera.MainCamera.rect;

                _players[i].Camera.MainCamera.targetDisplay = targetDisplay;
                _players[i].Camera.UICamera.targetDisplay = targetDisplay;
                //assign player number
                _players[i].PlayerControl.PlayerNumber = i;

                _players[i].PlayerController.Player = ReInput.players.GetPlayer(i);

                _players[i].PlayerUiManager.SetPosition(_players[i].Camera.MainCamera.rect, i);

                _players[i].PlayerUiManager.SetCharacter(GameManager.PlayerChoice[i]);

                _players[i].fog.FogOfWarPlane = FOG.transform;
                _players[i].fog.num = (i + 1);


                NotifyMessage += _players[i].PlayerUiManager.NotifyMessage;
            }

            #endregion

            #region Audio

            _audioSource[_currentAudioSource].clip = _backgroundMusicGathering;
            _audioSource[_currentAudioSource].Play();

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

        public void OpenDoor()
        {
            if (_doorOpen == false)
            {
                if (_audioSource.Length > 1)
                    StartCoroutine(AudioHelper.CrossFade(_audioSource[_currentAudioSource],
                        _audioSource[(_currentAudioSource + 1) % _audioSource.Length], _backgroundMusicLockdown, 5));
                _currentAudioSource = (_currentAudioSource + 1) % _audioSource.Length;
                _doorOpen = true;
            }
        }

        public IEnumerator OpenVault(Vault.OpenDoor callVault)
        {
            if (_audioSource.Length > 1)
                StartCoroutine(AudioHelper.CrossFade(_audioSource[_currentAudioSource],
                    _audioSource[(_currentAudioSource + 1) % _audioSource.Length], _backgroundMusicLockdown, 5));
            _currentAudioSource = (_currentAudioSource + 1) % _audioSource.Length;

            foreach (var player in _players)
            {
                player.PlayerUiManager.Siren.SetActive(true);
            }


            callVault();
            _vaultOpen = true;

            var time = _vaultTimer;
            do
            {
                time -= 1;
                foreach (var player in _players)
                {
                    player.PlayerUiManager.VaultTimer.text = time.ToString();
                }

                yield return new WaitForSeconds(1);
            } while (time > 0);

            foreach (var player in _players)
            {
                player.PlayerUiManager.VaultTimer.text = "";
            }
        }

        public void Notify(Vector3 position, NotifyType notifyType)
        {
            OnNotifty(new NotifyEventArgs
            {
                Position = position, NotifyType = notifyType
            });
        }

        protected virtual void OnNotifty(NotifyEventArgs e)
        {
            Notifty?.Invoke(this, e);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<PlayerControl>();
                PlayerLeaving[player.PlayerNumber] = true;
            }
        }


        public event NotifyMessageHandler NotifyMessage;

        public void NotifyPlayers(string message)
        {
            NotifyMessage?.Invoke(this, new NotifyMessageArgs()
            {
                Message = message
            });
            StartCoroutine(Delay(5));
        }

        private IEnumerator Delay(int i)
        {
            yield return new WaitForSeconds(i);
            NotifyMessage?.Invoke(this, new NotifyMessageArgs()
            {
                Message = ""
            });
        }
    }
}