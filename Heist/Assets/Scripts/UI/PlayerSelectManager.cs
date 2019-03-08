using System.Collections.Generic;
using Game;
using UnityEngine;

namespace UI
{
    public class PlayerSelectManager : MonoBehaviour
    {
        [SerializeField] private List<PlayerSelect> selection;

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