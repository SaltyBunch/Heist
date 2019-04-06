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
            var order = GameManager.GameManagerRef.Scores.OrderByDescending(x => x.PlayerScore).ToList();
            //get places from game manager
            for (int i = 0; i < 4; i++)
            {
                if (i < GameManager.NumPlayers)
                {
                    var playerModel = Instantiate(playerModels[(int) GameManager.PlayerChoice[order[i].PlayerNumber]],
                        _places[i]);
                    playerModel.SetMaterial(GameManager.GameManagerRef.Skins[order[i].PlayerNumber]);
                    playerModel.SetAnimation(i == 0 ? MenuAnim.Victory : MenuAnim.Defeat);
                    _placeTexts[i].ScoreText.text = order[i].PlayerScore.ToString();
                }
                else
                {
                    _places[i].gameObject.SetActive(false);
                }
            }
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
    }
}