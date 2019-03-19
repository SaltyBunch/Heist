using System.Collections.Generic;
using System.Linq;
using Game;
using UnityEngine;

namespace UI
{
    public class PlayerSelectableMenu : SelectableMenu
    {
        [SerializeField] private List<PlayerSelect> selection;

        [SerializeField] public List<Material> KingSkin;
        [SerializeField] public List<Material> JailbirdSkin;
        [SerializeField] public List<Material> ShadowSkin;
        [SerializeField] public List<Material> RoccoSkin;


        [SerializeField] public bool CaptureInput = false;

        private bool _entered;


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
            CaptureInput = false;
            _cancel.Invoke();
        }

        public override void Activate()
        {
            CaptureInput = true;
        }
    }
}