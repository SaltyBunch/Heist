using Game;
using Rewired;
using System.Collections.Generic;
using Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerSelect : MonoBehaviour
    {
        private bool _delay = true;
        [SerializeField] private string[] charas = {"King", "Shadow", "Jailbird", "Racoon"};
        [SerializeField] private TextMeshPro display;
        [SerializeField] private int player;

        [SerializeField] private List<PlayerModel> _playerModels;


        private Dictionary<Characters, List<Material>> CharacterSkins;


        [SerializeField] private int _playerSkinchoice;

        public bool ready;
        [SerializeField] private GameObject readyFX;
        [SerializeField] private int selection;

        [SerializeField] private PlayerSelectableMenu _playerSelectManager;

        private Rewired.Player _player;

        public int PlayerSkinchoice
        {
            get => _playerSkinchoice;
            set
            {
                value = ((value) % CharacterSkins[GameManager.PlayerChoice[player]].Count +
                         CharacterSkins[GameManager.PlayerChoice[player]].Count) %
                        CharacterSkins[GameManager.PlayerChoice[player]].Count;
                _playerSkinchoice = value;
                GameManager.Skins[player] = CharacterSkins[GameManager.PlayerChoice[player]][_playerSkinchoice];

                _playerModels[Selection].SetMaterial(CharacterSkins[GameManager.PlayerChoice[player]][_playerSkinchoice]);
            }
        }

        public int Selection
        {
            get { return selection; }
            set
            {
                value = (value % 4 + 4) % 4;
                if (selection != value)
                {
                    _playerModels[selection].gameObject.SetActive(false);
                    _playerModels[value].gameObject.SetActive(true);
                }

                selection = value;
            }
        }

        private void Awake()
        {
            CharacterSkins = new Dictionary<Characters, List<Material>>
            {
                {Characters.King, _playerSelectManager.KingSkin},
                {Characters.Jailbird, _playerSelectManager.JailbirdSkin},
                {Characters.Shadow, _playerSelectManager.ShadowSkin},
                {Characters.Raccoon, _playerSelectManager.RoccoSkin},
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
            readyFX.SetActive(ready);
            if (_playerSelectManager.CaptureInput)
            {
                if (_player.GetButtonDown("UIHorizontalRight") && !ready) Selection++;
                if (_player.GetButtonDown("UIHorizontalLeft") && !ready) Selection--;

                if (_player.GetButtonDown("UISubmit") && !ready) ready = true;

                if (ready && _player.GetButtonDown("UICancel")) ready = false;

                if (_player.GetButtonDown("UIVerticalUp")) PlayerSkinchoice++;
                if (_player.GetButtonDown("UIVerticalDown")) PlayerSkinchoice--;

                switch (Selection)
                {
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
}