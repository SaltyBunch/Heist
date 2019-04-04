using System.Collections.Generic;
using System.Linq;
using Character;
using UnityEngine;

namespace Game
{
    public class VictoryScene : MonoBehaviour
    {
        [SerializeField] private Transform[] _places;

        [SerializeField] private PlaceText[] _placeTexts;
        [SerializeField] private List<PlayerModel> playerModels;

        private void Start()
        {
            var order = GameManager.GameManagerRef.Scores.OrderByDescending(x => x.PlayerScore).ToList();
            //get places from game manager
            for (var i = 0; i < 4; i++)
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
}