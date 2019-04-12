using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Character;
using drone;
using Level;
using Pickup;
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
        Footstep,
        Attack
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
        public LayerMask EnvironmentLayer;
        [SerializeField] private AudioClip _backgroundMusicGathering;
        [SerializeField] private AudioClip _backgroundMusicInfiltration;
        [SerializeField] private AudioClip _backgroundMusicLockdown;
        [SerializeField] private float _endGameAtTime;
        [SerializeField] private Collider _gameEndArea;
        [SerializeField] private bool _gameOver;
        [SerializeField] private AudioSource[] _musicAudioSource;
        [SerializeField] private List<PickupSpawner> _pickupSpawners;
        [SerializeField] private List<PlayerGameObject> _playerGo;
        [SerializeField] private GameObject[] _spawnpoints;
        [SerializeField] private AudioSource _voiceAudioSource;
        [SerializeField] public DroneLoad droneSpawner;
        [SerializeField] public GameObject FOG;
        private int _currentAudioSource;
        private bool _doorOpen;
        private bool[] _playerLeaving;
        private float _time;
        private float _timeSinceVaultOpened;
        public bool vaultOpen => _vaultOpen;
        private bool _vaultOpen;
        private Player _most;

        private readonly List<Tuple<AudioClip, float>> _voiceQue = new List<Tuple<AudioClip, float>>();

        public Dictionary<NotifyType, float> NotificationRamge = new Dictionary<NotifyType, float>
        {
            {NotifyType.Dash, 10}, {NotifyType.Footstep, 5}, {NotifyType.TripTrap, 10}, {NotifyType.Attack, 10}
        };

        private bool _notify;
        private bool _escapeText;

        public PlayerGameObject[] Players { get; private set; }

        private float TimeSinceVaultOpened
        {
            get => _timeSinceVaultOpened;
            set
            {
                _timeSinceVaultOpened = value;
                if (_timeSinceVaultOpened > _endGameAtTime) AllPlayersLeft(false);
            }
        }

        public bool[] PlayerLeaving
        {
            get => _playerLeaving;
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

        public static float Time => LevelManagerRef._time;

        public void AllPlayersLeft(bool b)
        {
            CalculateScore();
            _gameOver = true;
            GameManager.GameManagerRef.EndGame();
        }

        public event NotifyEventHandler Notifty;

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

                if (!_vaultOpen && !_notify && _time > 240)
                {
                    _notify = true;
                    NotifyPlayers(TextHelper.LockDownNotif);
                }
                else if (!_vaultOpen && _time > 360)
                {
                    var vault = (Vault) FindObjectOfType(typeof(Vault));
                    vault.UseKey(new Dictionary<KeyType, bool>()
                    {
                        {KeyType.RedKey, true},
                        {KeyType.YellowKey, true}
                    }, null);
                }


                if (_vaultOpen)
                {
                    TimeSinceVaultOpened += 0.5f;
                    if (!_escapeText && TimeSinceVaultOpened + 60 > _endGameAtTime)
                    {
                        NotifyPlayers("<#f92a2a>60<#FFFFFF> Seconds Left To Escape");
                        _escapeText = true;
                    }
                }

                //place crown

                _most.PlayerUiManager.SetCrown(false);


                foreach (var player in _playerGo)
                {
                    if (_most == null || player.Player.Inventory.GoldAmount > _most.Inventory.GoldAmount)
                    {
                        _most = player.Player;
                    }
                }

                _most.PlayerUiManager.SetCrown(true);


            } while (!_gameOver);
        }

        public void CalculateScore()
        {
            GameManager.GameManagerRef.Scores = new List<Score>();
            foreach (var playerGO in Players)
            {
                var player = playerGO.Player;
                var playerNum = playerGO.PlayerControl.PlayerNumber;

                if (_playerLeaving[playerNum])
                    GameManager.GameManagerRef.Scores.Add(
                        new Score
                        {
                            GoldAmount = player.Inventory.GoldAmount,
                            TimesStunned = player.timesStunned,
                            PlayerNumber = playerGO.PlayerControl.PlayerNumber
                        });
                else
                    GameManager.GameManagerRef.Scores.Add(
                        new Score
                        {
                            GoldAmount = -1337,
                            TimesStunned = player.timesStunned,
                            PlayerNumber = playerGO.PlayerControl.PlayerNumber
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
                //todo place hazards
                pickupSpawner.Spawn();

            _gameEndArea.enabled = false;

            #endregion

            #region Player Setup

            Players = new PlayerGameObject[numPlayers];

            ShuffleSpawns();


            for (var i = 0; i < numPlayers; i++)
            {
                if (i != 0 && i % playersPerDisplay == 0)
                    targetDisplay++;
                Players[i] = Instantiate(_playerGo[(int) GameManager.PlayerChoice[i]]);

                //put player on spawnpoint
                Players[i].transform.position = _spawnpoints[i].transform.position;

                //set screen region
                switch (playersOnDisplay[targetDisplay])
                {
                    case 1:
                        Players[i].Camera.MainCamera.rect = new Rect
                        {
                            x = 0,
                            y = 0,
                            width = 1,
                            height = 1
                        };
                        break;
                    case 2:
                        Players[i].Camera.MainCamera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = 0,
                            width = 0.5f,
                            height = 1
                        };
                        break;
                    case 3:
                        if (i == 1)
                        {
                            Players[i].Camera.MainCamera.rect = new Rect
                            {
                                x = i % 2 * 0.5f,
                                y = 0,
                                width = 0.5f,
                                height = 1
                            };
                        }
                        else
                        {
                            Players[i].Camera.MainCamera.rect = new Rect
                            {
                                x = i % 2 * 0.5f,
                                y = Mathf.Abs(1 - i / 2) * 0.5f,
                                width = 0.5f,
                                height = 0.5f
                            };
                        }

                        break;
                    case 4:
                        Players[i].Camera.MainCamera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = Mathf.Abs(1 - i / 2) * 0.5f,
                            width = 0.5f,
                            height = 0.5f
                        };
                        break;
                }

                Players[i].Camera.MainCamera.targetDisplay = targetDisplay;
                //assign player number
                Players[i].PlayerControl.PlayerNumber = i;

                Players[i].PlayerController.Player = ReInput.players.GetPlayer(i);

                Players[i].PlayerUiManager.SetPosition(Players[i].Camera.MainCamera.rect, i);

                Players[i].PlayerUiManager.SetCharacter(GameManager.PlayerChoice[i]);

                Players[i].fog.FogOfWarPlane = FOG.transform;
                Players[i].fog.num = i + 1;

                NotifyMessage += Players[i].PlayerUiManager.NotifyMessage;
            }

            _playerLeaving = new bool[numPlayers];

            #endregion

            #region Drone Setup

            var players = new List<Player>();
            foreach (var player in Players) players.Add(player.Player);

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
                        _backgroundMusicInfiltration, 5));
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

            _musicAudioSource[_currentAudioSource].loop = false;

            foreach (var player in Players) player.PlayerUiManager.Siren.SetActive(true);


            callVault();
            _vaultOpen = true;
            _gameEndArea.enabled = true;

            NotifyPlayers("Collect gold and escape the bank");

            Func<int> time = () => (int)( _endGameAtTime - TimeSinceVaultOpened);
            do
            {                
                foreach (var player in Players) player.PlayerUiManager.VaultTimer.text = time.Invoke().ToString();
                yield return new WaitForSeconds(1);
            } while (time.Invoke() > 0);



            foreach (var player in Players) player.PlayerUiManager.VaultTimer.text = "";
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
            NotifyMessage?.Invoke(this, new NotifyMessageArgs
            {
                Message = message
            });
            StartCoroutine(Delay(5));
        }

        private IEnumerator Delay(int i)
        {
            yield return new WaitForSeconds(i);
            NotifyMessage?.Invoke(this, new NotifyMessageArgs
            {
                Message = ""
            });
        }

        public void SetKeyPickedUp(KeyType keyPickupKey)
        {
            foreach (var player in Players) player.PlayerUiManager.SetKeyPickedUp(keyPickupKey);
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
                        if (_time - current.Item2 < 0.33f) // less then 1/3 secs since request
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