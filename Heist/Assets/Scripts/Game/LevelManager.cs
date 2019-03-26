using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Character;
using Level;
using Pickup;
using Rewired;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
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

        [SerializeField] private AudioSource[] _musicAudioSource;
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
        [SerializeField] public drone.DroneLoad droneSpawner;

        [SerializeField] private List<PickupSpawner> _pickupSpawners;

        
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

        [SerializeField] private Collider _gameEndArea;
        private bool[] _playerLeaving;

        public bool[] PlayerLeaving
        {
            get { return _playerLeaving; }
            set
            {
                _playerLeaving = value;
                for (var i = 0; i < _playerLeaving.Length; i++)
                {
                    var b = _playerLeaving[i];
                    if (!b) break;
                    if (i + 1 == _playerLeaving.Length) AllPlayersLeft(true);
                }

            }
        }

        public void AllPlayersLeft(bool b)
        {
            CalculateScore();
            _gameOver = true;
            GameManager.GameManagerRef.EndGame();
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
        [SerializeField] private AudioSource _voiceAudioSource;
        [SerializeField] private bool _gameOver;
        private List<Tuple<AudioClip, float>> _voiceQue = new List<Tuple<AudioClip, float>>();

        private void Awake()
        {
            if (LevelManagerRef == null) LevelManagerRef = this;

            _spawnpoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

            _time = 0;
        }

        private void Start()
        {
            //var players = Rewired.ReInput.players.playerCount;
            var players = GameManager.NumPlayers;
            //InitGame(4);

            StartCoroutine(LevelTimer());
            StartCoroutine(VoiceLineMonitor());
        }

        private IEnumerator LevelTimer()
        {
            do
            {
                yield return new WaitForSeconds(0.5f);
                _time += 0.5f;
                if (_vaultOpen) TimeSinceVaultOpened += 0.5f;
            } while (!_gameOver);
        }

        public void CalculateScore()
        {
            GameManager.GameManagerRef.Scores = new List<Score>();
            foreach (var playerGO in _players)
            {
                var player = playerGO.Player;
                var playerNum = playerGO.PlayerControl.PlayerNumber;

                if (_playerLeaving[playerNum])
                {
                    GameManager.GameManagerRef.Scores.Add(
                        new Score()
                        {
                            GoldAmount = player.Inventory.GoldAmount,
                            TimesStunned = player.timesStunned,
                            PlayerNumber = playerGO.PlayerControl.PlayerNumber,
                        });
                }
                else
                {
                    GameManager.GameManagerRef.Scores.Add(
                        new Score()
                        {
                            GoldAmount = 0,
                            TimesStunned = player.timesStunned,
                            PlayerNumber = playerGO.PlayerControl.PlayerNumber,
                        });
                }
            }
        }

        public void InitGame(int numPlayers)
        {
        
            var displays = 1;
            if (GameManager.GameManagerRef.UseMultiScreen)
            {
                // Number of displays
#if UNITY_EDITOR || UNITY_EDITOR_64
                displays = 1;
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
                    displays = j;
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

            foreach (var pickupSpawner in _pickupSpawners)
            {
                //todo place hazards
                pickupSpawner.Spawn();
            }

            _gameEndArea.enabled = false;

            #endregion

            #region Player Setup

            _players = new PlayerGameObject[numPlayers];

            ShuffleSpawns();


            for (var i = 0; i < numPlayers; i++)
            {
                if (i != 0 && i % playersPerDisplay == 0)
                    targetDisplay++;
                _players[i] = Instantiate(_playerGo[(int) GameManager.PlayerChoice[i]]);
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

            _playerLeaving = new bool[numPlayers];

            #endregion

            #region Drone Setup

            var players = new List<Player>();
            foreach (var player in Players)
            {
                players.Add(player.Player);
            }

            //todo drone setup
            droneSpawner.Begin(players);

            #endregion

            #region Audio

            _musicAudioSource[_currentAudioSource].clip = _backgroundMusicGathering;
            _musicAudioSource[_currentAudioSource].Play();

            #endregion
            
            NotifyPlayers("Infiltrate the vault");
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
                if (_musicAudioSource.Length > 1)
                    StartCoroutine(AudioHelper.CrossFade(_musicAudioSource[_currentAudioSource],
                        _musicAudioSource[(_currentAudioSource + 1) % _musicAudioSource.Length],
                        _backgroundMusicGathering, 5));
                _currentAudioSource = (_currentAudioSource + 1) % _musicAudioSource.Length;
                _doorOpen = true;
            }
        }

        public IEnumerator OpenVault(Vault.OpenDoor callVault)
        {
            if (_musicAudioSource.Length > 1)
                StartCoroutine(AudioHelper.CrossFade(_musicAudioSource[_currentAudioSource],
                    _musicAudioSource[(_currentAudioSource + 1) % _musicAudioSource.Length], _backgroundMusicLockdown,
                    5));
            _currentAudioSource = (_currentAudioSource + 1) % _musicAudioSource.Length;

            foreach (var player in _players)
            {
                player.PlayerUiManager.Siren.SetActive(true);
            }


            callVault();
            _vaultOpen = true;
            _gameEndArea.enabled = true;

            NotifyPlayers("Collect gold and escape the bank");
            
            var time = _endGameAtTime;
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
                PlayerLeaving = PlayerLeaving;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var player = other.GetComponentInParent<PlayerControl>();
                PlayerLeaving[player.PlayerNumber] = false;
                PlayerLeaving = PlayerLeaving;
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

        public void SetKeyPickedUp(KeyType keyPickupKey)
        {
            foreach (var player in _players)
            {
                player.PlayerUiManager.SetKeyPickedUp(keyPickupKey);
            }
        }

        public void PlayVoiceLine(AudioClip voiceLine)
        {
            //if (voiceLine == null) return;
            if (!_voiceAudioSource.isPlaying)
            {
                _voiceAudioSource.clip = voiceLine;
                _voiceAudioSource.Play();
            }
            else
            {
                _voiceQue.Add(new Tuple<AudioClip, float>(voiceLine, _time));
            }
        }

        private IEnumerator VoiceLineMonitor()
        {
            do
            {
                //
                if (!_voiceAudioSource.isPlaying && _voiceQue.Count > 0)
                {
                    var current = _voiceQue.First();
                    if (current != null)
                    {
                        if (_time - current.Item2 < 2) // less then 5 secs since request
                        {
                            PlayVoiceLine(current.Item1);
                            _voiceQue.RemoveAt(0);
                        }
                        else
                        {
                            _voiceQue.RemoveAt(0);
                        }
                    }
                }

                //
                yield return null;
            } while (!_gameOver);
        }
    }
}