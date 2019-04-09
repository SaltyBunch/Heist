using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Character;
using Rewired;
using UI;
using UnityEngine;

namespace Game
{
    public class VictoryScene : MonoBehaviour
    {
        [SerializeField] private List<PlayerModel> playerModels;

        [SerializeField] private Transform[] _places;

        [SerializeField] private PlaceText[] _placeTexts;
        private static MenuManager.Control _victoryControl;
        private static bool _exiting;

        [SerializeField] private List<AudioClip> _victory;
        [SerializeField] private List<AudioClip> _defeat;
        [SerializeField] private AudioSource _musicAudio;
        [SerializeField] private AudioSource _voiceAudio;
        [SerializeField] private AudioClip _victoryClip;
        [SerializeField] private AudioClip _defeatClip;

        public static MenuManager.Control VictoryControl
        {
            get { return _victoryControl; }
            set
            {
                if (!Equals(value, _victoryControl))
                {
                    if (value.Start && !_victoryControl.Start && !_exiting)
                    {
                        _exiting = true;
                        GameManager.GameManagerRef.ToMain();
                    }
                }

                _victoryControl = value;
            }
        }

        private void Start()
        {
            StartCoroutine(PlaceCharacters());
        }

        private void Update()
        {
            var control = VictoryControl;

            foreach (var player in ReInput.players.Players)
            {
                control.Start = control.Start || player.GetButtonDown("Pause");
            }

            VictoryControl = control;
        }


        IEnumerator PlaceCharacters()
        {
            var order = GameManager.GameManagerRef.Scores.OrderByDescending(x => x.PlayerScore).ToList();
            //get places from game manager
            for (int i = 0; i < GameManager.NumPlayers; i++)
            {
                _places[i].gameObject.SetActive(true);
                var playerModel = Instantiate(playerModels[(int) GameManager.PlayerChoice[order[i].PlayerNumber]],
                    _places[i]);
                playerModel.SetMaterial(GameManager.GameManagerRef.Skins[order[i].PlayerNumber]);
                var victory = i == 0 && order[i].PlayerScore > 0;
                playerModel.SetAnimation(victory ? MenuAnim.Victory : MenuAnim.Defeat);
                _placeTexts[i].ScoreText.text = order[i].PlayerScore.ToString();
                
                _musicAudio.clip = victory ? _victoryClip : _defeatClip;
                _musicAudio.Play();

                _voiceAudio.clip = i == 0
                    ? _victory[(int) GameManager.PlayerChoice[order[i].PlayerNumber]]
                    : _defeat[(int) GameManager.PlayerChoice[order[i].PlayerNumber]];
                _voiceAudio.Play();
                do
                {
                    yield return null;
                } while (_voiceAudio.isPlaying);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}