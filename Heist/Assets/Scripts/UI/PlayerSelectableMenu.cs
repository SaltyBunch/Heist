using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UI
{
    public class PlayerSelectableMenu : SelectableMenu
    {
        private bool _entered;


        [SerializeField] public bool CaptureInput;
        [SerializeField] public List<Material> JailbirdSkin;

        [SerializeField] public List<Material> KingSkin;
        [SerializeField] public List<Material> RoccoSkin;
        [SerializeField] private List<PlayerSelect> selection;
        [SerializeField] public List<Material> ShadowSkin;


        private void Start()
        {
            GameManager.PlayerChoice = new Characters[GameManager.NumPlayers];
        }

        public void Update()
        {
            if (!_entered && selection.TrueForAll(s => s.ready || !s.gameObject.activeSelf))
            {
                GameManager.GameManagerRef.EnterGame();
                _entered = true;
            }
        }

        public override void Right()
        {
        }

        public override void Left()
        {
        }

        public override void Up()
        {
        }

        public override void Down()
        {
        }

        public override void Submit()
        {
        }

        public override void Cancel()
        {
        }

        public override void Activate()
        {
            CaptureInput = true;
        }

        public void Exit()
        {
            if (selection.TrueForAll(s => !s.ready || !s.gameObject.activeSelf))
            {
                CaptureInput = false;
                _cancel.Invoke();
            }
        }
    }
}