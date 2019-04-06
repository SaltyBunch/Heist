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
        private string[] _characterNames = {"King", "Jailbird", "Shadow", "Rocco"};
        [SerializeField] private TextMeshPro display;
        [SerializeField] private int player;

        [SerializeField] private List<PlayerModel> _playerModels;


        private Dictionary<Characters, List<Material>> CharacterSkins;


        [SerializeField] private int _playerSkinchoice;

        public bool ready;
        [SerializeField] private GameObject readyFX;
        [SerializeField] private int selection;

        [SerializeField] private PlayerSelectableMenu _playerSelectManager;

        [SerializeField] private TextMeshPro _health, _speed, _dexterity;

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
                GameManager.GameManagerRef.Skins[player] =
                    CharacterSkins[GameManager.PlayerChoice[player]][_playerSkinchoice];

                _playerModels[Selection]
                    .SetMaterial(CharacterSkins[GameManager.PlayerChoice[player]][_playerSkinchoice]);
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
                    _playerModels[value].SetAnimation(MenuAnim.Select);

                }

                selection = value;

                display.text = _characterNames[Selection];
                GameManager.PlayerChoice[player] = (Characters) Selection;
                _speed.text = GameManager.CharacterStats[GameManager.PlayerChoice[player]].Speed.ToString();
                _health.text = GameManager.CharacterStats[GameManager.PlayerChoice[player]].Health.ToString();
                _dexterity.text = GameManager.CharacterStats[GameManager.PlayerChoice[player]].Dexterity.ToString();
                PlayerSkinchoice %= CharacterSkins[GameManager.PlayerChoice[player]].Count;
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


            if (GameManager.NumPlayers <= player) gameObject.SetActive(false);
            else gameObject.SetActive(true);

            if (player < GameManager.NumPlayers)
            {
                _player = ReInput.players.GetPlayer(player);

                Selection = player;
                PlayerSkinchoice = 0;
            }
        }

        // Update is called once per frame
        private void Update()
        {
            readyFX.SetActive(ready);
            if (_playerSelectManager.CaptureInput)
            {
                if (_player.GetButtonDown("UIHorizontalLeft") && !ready) Selection--;
                if (_player.GetButtonDown("UIHorizontalRight") && !ready) Selection++;

                if (_player.GetButtonDown("UISubmit") && !ready) ready = true;

                if (ready && _player.GetButtonDown("UICancel")) ready = false;
                else if (!ready && _player.GetButtonDown("UICancel")) _playerSelectManager.Exit();

                if (_player.GetButtonDown("UIVerticalUp")) PlayerSkinchoice++;
                if (_player.GetButtonDown("UIVerticalDown")) PlayerSkinchoice--;
            }
        }
    }
}