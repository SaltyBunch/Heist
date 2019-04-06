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
            public bool Start;
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
                        MenuManagerRef.CurrentMenu.Right();
                    if (value.Left && !_menuControl.Left)
                        MenuManagerRef.CurrentMenu.Left();
                    if (value.Up && !_menuControl.Up)
                        MenuManagerRef.CurrentMenu.Up();
                    if (value.Down && !_menuControl.Down)
                        MenuManagerRef.CurrentMenu.Down();
                    if (value.Submit && !_menuControl.Submit)
                        MenuManagerRef.CurrentMenu.Submit();
                    if (value.Cancel && !_menuControl.Cancel)
                        MenuManagerRef.CurrentMenu.Cancel();
                }
            }
        }

        public SelectableMenu CurrentMenu
        {
            get { return _currentMenu; }
            set
            {
                _currentMenu = value;
                _currentMenu.Activate();
            }
        }

        [SerializeField] private SelectableMenu _currentMenu;
        private static Control _menuControl;
        [SerializeField] private Animator _menuAnimator;
        [SerializeField] private Animator _vaultAnimator;


        [SerializeField] private SelectableMenu _mainMenu;
        [SerializeField] private SelectableMenu _optionsMenu;
        [SerializeField] private SelectableMenu _playerSelect;
        [SerializeField] private SelectableMenu _credits;
        [SerializeField] private SelectableMenu _controls;
        [SerializeField] private SelectableMenu _sound;
        private bool _input = true;

        private void Start()
        {
            MenuManagerRef = this;
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
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void GoToMainFromOptions()
        {
            _menuAnimator.SetTrigger("ToMainFromOptions");
            CurrentMenu = _mainMenu;
        }

        public void GoToMainFromPlayerSelect()
        {
            _vaultAnimator.SetTrigger("Close");

            _menuAnimator.SetTrigger("ToMainFromPlayerSelect");
            CurrentMenu = _mainMenu;
        }

        public void GoToOptions()
        {
            _menuAnimator.SetTrigger("GoToOptions");
            CurrentMenu = _optionsMenu;
        }

        public void GoToCharSelect()
        {
            _vaultAnimator.SetTrigger("Open");

            _menuAnimator.SetTrigger("GoToPlayerSelect");
            CurrentMenu = _playerSelect;
        }

        public void GoToCredits()
        {
            _menuAnimator.SetTrigger("GoToCredits");
            _credits.gameObject.SetActive(true);
            CurrentMenu = _credits;
        }

        public void GoToOptionsFromCredits()
        {
            _menuAnimator.SetTrigger("GoFromCredits");
            CurrentMenu = _optionsMenu;
            _credits.gameObject.SetActive(false);
        }

        public void GoToControls()
        {
            _menuAnimator.SetTrigger("GoToCredits");
            _controls.gameObject.SetActive(true);
            CurrentMenu = _controls;
        }

        public void GoToOptionsFromControls()
        {
            _menuAnimator.SetTrigger("GoFromCredits");
            CurrentMenu = _optionsMenu;
            _controls.gameObject.SetActive(false);
        }

        public void GoToAudio()
        {
            _menuAnimator.SetTrigger("GoToCredits");
            _sound.gameObject.SetActive(true);
            CurrentMenu = _sound;
        }

        public void GoToOptionsFromAudio()
        {
            _menuAnimator.SetTrigger("GoFromCredits");
            CurrentMenu = _optionsMenu;
            _sound.gameObject.SetActive(false);
        }

        public void Empty()
        {
        }
    }
}