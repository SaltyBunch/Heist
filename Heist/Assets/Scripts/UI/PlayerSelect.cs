using Game;
using Rewired;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerSelect : MonoBehaviour
    {
        private bool _delay = true;
        [SerializeField] private string[] charas = { "King", "Shadow", "Jailbird", "Racoon" };
        [SerializeField] private Text display;
        [SerializeField] private Menu mm;
        [SerializeField] private int player;


        private Dictionary<Characters, List<Material>> CharacterSkins;


        [SerializeField] private int _playerSkinchoice;

        public bool ready;
        [SerializeField] private GameObject readyFX;
        [SerializeField] private int selection;

        [SerializeField] private PlayerSelectManager _playerSelectManager;

        private Rewired.Player _player;

        public int PlayerSkinchoice
        {
            get => _playerSkinchoice;
            set
            {
                _playerSkinchoice = value;
                GameManager.Skins[player] = CharacterSkins[GameManager.PlayerChoice[player]][_playerSkinchoice];

                //todo update player models
            }
        }

        private void Awake()
        {
            CharacterSkins = new Dictionary<Characters, List<Material>>
            {
                {Characters.King, _playerSelectManager.KingSkin },
                {Characters.Jailbird, _playerSelectManager.JailbirdSkin },
                {Characters.Shadow, _playerSelectManager.ShadowSkin },
                {Characters.Raccoon, _playerSelectManager.RoccoSkin },
            };

        }

        private void OnEnable()
        {
            if (GameManager.NumPlayers <= player) gameObject.SetActive(false);
            else gameObject.SetActive(true);

            _player = ReInput.players.GetPlayer(player);
        }

        // Update is called once per frame
        private void Update()
        {
            readyFX.SetActive(false);
            if (_player.GetAxisDelta("UIHorizontal") > 0.5f && _player.GetAxis("UIHorizontal") > 0.5f && !ready) selection++;
            if (_player.GetAxisDelta("UIHorizontal") < -0.5f && _player.GetAxis("UIHorizontal") < -0.5f && !ready) selection--;

            if (_player.GetButtonDown("UISubmit") && !ready) ready = true;

            if (!ready && _player.GetButtonDown("UICancel")) mm.ExitPlayerSelect();

            if (ready && _player.GetButtonDown("UICancel")) ready = false;

            readyFX.SetActive(ready);

            selection %= 4;


            if (_player.GetAxisDelta("UIVertical") > 0.5f && _player.GetAxis("UIVertical") > 0.5f) PlayerSkinchoice = (PlayerSkinchoice + 1) % CharacterSkins[GameManager.PlayerChoice[player]].Count;
            if (_player.GetAxisDelta("UIVertical") < -0.5f && _player.GetAxis("UIVertical") < -0.5f) PlayerSkinchoice = (PlayerSkinchoice + 1) % CharacterSkins[GameManager.PlayerChoice[player]].Count;

            switch (selection)
            {
                case -1:
                    selection = 3;
                    break;
                case 4:
                    selection = 0;
                    break;
                case 0:
                    display.text = charas[0];
                    GameManager.PlayerChoice[player] = Characters.King;
                    PlayerSkinchoice %= CharacterSkins[GameManager.PlayerChoice[player]].Count;
                    break;
                case 1:
                    display.text = charas[1];
                    GameManager.PlayerChoice[player] = Characters.Shadow;
                    PlayerSkinchoice %= CharacterSkins[GameManager.PlayerChoice[player]].Count;
                    break;
                case 2:
                    display.text = charas[2];
                    GameManager.PlayerChoice[player] = Characters.Jailbird;
                    PlayerSkinchoice %= CharacterSkins[GameManager.PlayerChoice[player]].Count;
                    break;
                case 3:
                    display.text = charas[3];
                    GameManager.PlayerChoice[player] = Characters.Raccoon;
                    PlayerSkinchoice %= CharacterSkins[GameManager.PlayerChoice[player]].Count;
                    break;
            }
        }
    }
}