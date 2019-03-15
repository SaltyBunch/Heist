using Rewired;
using UnityEngine;
using UnityEngine.Events;

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
        [SerializeField] private Animator _menuAnimator;
        private UnityEvent SwitchToOptions = new UnityEvent();
        private UnityEvent SwitchToMain = new UnityEvent();


        [SerializeField] private SelectableMenu _mainMenu;
        [SerializeField] private SelectableMenu _optionsMenu;
        private bool _input = true;

        private void Start()
        {
            MenuManagerRef = this;
            SwitchToOptions.AddListener(() =>
            {
                _currentMenu = _optionsMenu;
                _input = true;
            });
            SwitchToMain.AddListener(() =>
            {
                _currentMenu = _mainMenu;

                _input = true;
            });
        }


        private void Update()
        {
            //if (_menuAnimator.IsInTransition(0))
            //{
                //j - nick C
                //Get control
                var control = MenuControl;

                foreach (var player in ReInput.players.Players)
                {
                    control.Right = control.Right || player.GetButtonDown("UIHorizontalRight");
                    control.Left = control.Left || player.GetButtonDown("UIHorizontalLeft");
                    control.Up = control.Up || player.GetButtonDown("UIVerticalUp");
                    control.Down = control.Down || player.GetButtonDown("UIVerticalDown");
                    control.Submit = control.Submit || player.GetButtonDown("UISubmit");
                    control.Cancel = control.Cancel || player.GetButtonDown("UICancel");
                }

                MenuControl = control;
          //  }
        }

        public void ExitGame()
        {
        }

        public void GoToMainFromOptions()
        {
            _menuAnimator.SetTrigger("ToMainFromOptions");
            _currentMenu = _mainMenu;
        }

        public void GoToMainFromPlayerSelect()
        {
            _menuAnimator.SetTrigger("ToMainFromPlayerSelect");
            _currentMenu = _mainMenu;
        }

        public void GoToOptions()
        {
            _menuAnimator.SetTrigger("GoToOptions");
            _currentMenu = _optionsMenu;
        }

        public void GoToCharSelect()
        {
            _menuAnimator.SetTrigger("GoToPlayerSelect");
        }
    }
}