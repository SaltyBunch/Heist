using System;
using System.Collections;
using System.Collections.Generic;
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

    [RequireComponent(typeof(AudioSource), typeof(FloorManager))]
    public class LevelManager : MonoBehaviour
    {
        public delegate void NotifyEventHandler(object sender, NotifyEventArgs e);

        public static LevelManager LevelManagerRef;

        private AudioSource[] _audioSource;
        [SerializeField] private AudioClip _backgroundMusicGathering;

        [SerializeField] private AudioClip _backgroundMusicInfiltration;
        [SerializeField] private AudioClip _backgroundMusicLockdown;
        private int _currentAudioSource;

        [SerializeField] private int _goldMultiplier = 10;

        [SerializeField] private PlayerGameObject _playerGo;

        private PlayerGameObject[] _players;

        [SerializeField] private GameObject[] _spawnpoints;
        [SerializeField] private int _stunMultiplier = 100;

        private float _time;

        [SerializeField] private float _vaultTimer;

        public Dictionary<NotifyType, float> NotificationRamge = new Dictionary<NotifyType, float>
        {
            {NotifyType.Dash, 10}, {NotifyType.Footstep, 5}, {NotifyType.TripTrap, 100}
        };

        public static float Time => LevelManagerRef._time;

        [SerializeField] public FloorManager FloorManager { get; }

        public event NotifyEventHandler Notifty;

        private void Awake()
        {
            if (LevelManagerRef == null) LevelManagerRef = this;

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
                        _players[i].Camera.MainFloorCamera.rect = new Rect
                        {
                            x = 0,
                            y = 0,
                            width = 1,
                            height = 1
                        };
                        break;
                    case 2:
                        _players[i].Camera.MainFloorCamera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = 0,
                            width = 0.5f,
                            height = 1
                        };
                        break;
                    case 3:
                    case 4:
                        _players[i].Camera.MainFloorCamera.rect = new Rect
                        {
                            x = i % 2 * 0.5f,
                            y = Mathf.Abs(1 - i / 2) * 0.5f,
                            width = 0.5f,
                            height = 0.5f
                        };
                        break;
                }

                _players[i].Camera.MainFloorCamera.targetDisplay = targetDisplay;
                //assign player number
                _players[i].PlayerControl.PlayerNumber = i;

                _players[i].PlayerControl.Player = ReInput.players.GetPlayer(i);
            }

            for (var i = 0; i < numPlayers; i++)
                UIManager.UiManagerRef.SetFace((int) GameManager.PlayerChoice[i], i);

            #endregion

            #region Audio

            _audioSource[_currentAudioSource].clip = _backgroundMusicInfiltration;
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

        public IEnumerator OpenVault(Vault.OpenDoor callVault)
        {
            if (_audioSource.Length > 1)
                StartCoroutine(AudioHelper.CrossFade(_audioSource[_currentAudioSource],
                    _audioSource[(_currentAudioSource + 1) % _audioSource.Length], _backgroundMusicLockdown, 5));
            _currentAudioSource = (_currentAudioSource + 1) % _audioSource.Length;
            yield return new WaitForSeconds(_vaultTimer);
            callVault();
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
    }
}