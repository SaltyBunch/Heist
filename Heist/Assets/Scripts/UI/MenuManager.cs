using Rewired;
using UnityEngine;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        public struct Control
        {
            public bool Right;
            public bool Left;
            public bool Up;
            public bool Down;
            public bool Submit;
            public bool Cancel;
        }

        public static MenuManager MenuManagerRef;

        public static Control MenuControl
        {
            get { return _menuControl; }
            set
            {
                if (!Equals(value, _menuControl))
                {
                    if (value.Right && !_menuControl.Right)
                        MenuManagerRef._currentMenu.Right();
                    if (value.Left && !_menuControl.Left)
                        MenuManagerRef._currentMenu.Left();
                    if (value.Up && !_menuControl.Up)
                        MenuManagerRef._currentMenu.Up();
                    if (value.Down && !_menuControl.Down)
                        MenuManagerRef._currentMenu.Down();
                    if (value.Submit && !_menuControl.Submit)
                        MenuManagerRef._currentMenu.Submit();
                    if (value.Cancel && !_menuControl.Cancel)
                        MenuManagerRef._currentMenu.Cancel();
                }
            }
        }

        [SerializeField] private SelectableMenu _currentMenu;
        private static Control _menuControl;

        private void Start()
        {
            MenuManagerRef = this;
        }


        private void Update()
        {
            //j - nick C
            //Get control
            var right = _menuControl.Right;
            var left = _menuControl.Left;
            var down = _menuControl.Down;
            var up = _menuControl.Up;
            
            foreach (var player in ReInput.players.Players)
            {
                right = right || player.GetButtonDown("UIHorizontalRight");
                left =  left ||player.GetButtonDown("UIHorizontalLeft");
                up = up || player.GetButtonDown("UIVerticalUp");
                down = down || player.GetButtonDown("UIVerticalDown");
            }

            var control = MenuControl;
            control.Right = right;
            control.Left = left;
            control.Up = up;
            control.Down = down;
            MenuControl = control;
        }
    }
}