using Character;
using UnityEngine;

namespace Manager
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager GameMananger;

        private void Awake()
        {
            if (GameMananger == null || GameMananger != this) GameMananger = this;
            else Destroy(this.gameObject);
        }

        public static int CalculateScore(Player player)
        {
            var score = player.Inventory.GoldAmount * 100; 
            //todo add more sources of score
            return score;
        }
    }
}