using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UI
{
    public class PlayerSelectManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerSelect> selection;

        private void OnEnable()
        {
            GameManager.PlayerChoice = new Characters[GameManager.NumPlayers];
        }

        public void Update()
        {
            if (selection.TrueForAll(s => s.ready || !s.gameObject.activeSelf))
            {
                GameManager.GameManagerRef.EnterGame();
                this.gameObject.SetActive(false);
            }
        }
    }
}