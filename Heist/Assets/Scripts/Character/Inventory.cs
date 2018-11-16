using UnityEngine;

namespace Character
{
    public class Inventory : MonoBehaviour
    {
        public int GoldAmount;

        private Hazard.Hazard _hazard;

        internal Hazard.Hazard Hazard
        {
            get { return _hazard; }
            set //todo
            {
                if (value != _hazard)
                {
                    //destroy current hazard

                    //set _hazard to value

                    //bind value to visual
                }
            }
        }

        private Weapon.Weapon _weapon;

        internal Weapon.Weapon Weapon
        {
            get { return _weapon; }
            set //todo
            {
                if (value != _weapon)
                {
                    //destroy current weapon

                    //set _weapon to value

                    //bind value to visual
                }
            }
        }
    }
}