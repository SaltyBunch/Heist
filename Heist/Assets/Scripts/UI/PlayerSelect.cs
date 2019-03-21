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
                }

                selection = value;
                
                display.text = _characterNames[Selection];
                GameManager.PlayerChoice[player] = (Characters) Selection;
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
        }

        private void OnEnable()
        {
            Selection = 0;
        
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
                
            }
        }
    }
}