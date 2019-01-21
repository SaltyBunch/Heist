using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.EventSystems;
using UnityEditor;

namespace UI
{
    enum Menus
    {
        Main, Selection, Options, Controls, Audio, Credits
    }

    public class Menu : MonoBehaviour
    {
        [SerializeField] EventSystem ES;

        [SerializeField] GameObject mainMenu;
        [SerializeField] GameObject mainMenuButton;
        [SerializeField] GameObject SelectionMenu; 
        //[SerializeField] GameObject SelectionMenuButton;
        [SerializeField] GameObject optionsMenu;
        [SerializeField] GameObject optionsButton;
        [SerializeField] GameObject controlsMenu;
        [SerializeField] GameObject audioMenu;
        [SerializeField] GameObject audioButton;
        [SerializeField] GameObject creditsMenu;

        private Menus currentMenu;

        // Start is called before the first frame update
        void Start()
        {

            LoadUIControls();
            currentMenu = Menus.Main;
        }

        private void Update()
        {
            foreach(var player in ReInput.players.AllPlayers)
            {
                if (player.GetButtonDown("UICancel"))
                {
                    switch (currentMenu)
                    {
                        case Menus.Controls :
                            ExitcontrolsMenu();
                            break;
                        case Menus.Audio:
                            ExitAudioMenu();
                            break;
                        case Menus.Credits:
                            ExitCreditsMenu();
                            break;
                        case Menus.Options:
                            ExitOptionsMenu();
                            break;
                    }
                    break;
                }
            }
        }

        public void LoadUIControls()
        {
            foreach (var player in ReInput.players.AllPlayers)
            {
                // Re-load the keyboard maps that were assigned to this Player in the Rewired Input Manager
                player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
                player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);

                // Load joysticks maps in each joystick in the "UI" category and "Default" layout and set it to be enabled on start
                foreach (Joystick joystick in player.controllers.Joysticks)
                {
                    player.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Menu", "Default");
                    player.controllers.maps.LoadMap(ControllerType.Joystick, player.id, "Menu", "Default");
                }
            }
        }

        public void LoadInGameControls()
        {
            foreach (var player in ReInput.players.AllPlayers)
            {
                // Re-load the keyboard maps that were assigned to this Player in the Rewired Input Manager
                player.controllers.maps.LoadDefaultMaps(ControllerType.Keyboard);
                player.controllers.maps.LoadDefaultMaps(ControllerType.Joystick);

                // Load joysticks maps in each joystick in the "UI" category and "Default" layout and set it to be enabled on start
                foreach (Joystick joystick in player.controllers.Joysticks)
                {
                    player.controllers.maps.LoadMap(ControllerType.Keyboard, 0, "Default", "Default");
                    player.controllers.maps.LoadMap(ControllerType.Joystick, player.id, "Default", "Default");
                }
            }
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else 
            Application.Quit();
#endif
        }

        public void EnterPlayerSelect()
        {
            mainMenuButton = ES.currentSelectedGameObject;
            SelectionMenu.SetActive(true);
            mainMenu.SetActive(false);
            ES.SetSelectedGameObject(null);
            currentMenu = Menus.Selection;
        }
        public void ExitPlayerSelect()
        {
            SelectionMenu.SetActive(false);
            mainMenu.SetActive(true);
            ES.SetSelectedGameObject(mainMenuButton);
            currentMenu = Menus.Main;
        }

        public void EnterOptionsMenu()
        {
            mainMenuButton = ES.currentSelectedGameObject;
            optionsMenu.SetActive(true);
            mainMenu.SetActive(false);
            ES.SetSelectedGameObject(optionsButton);
            currentMenu = Menus.Options;
        }

        public void ExitOptionsMenu()
        {
            optionsButton = ES.currentSelectedGameObject;
            optionsMenu.SetActive(false);
            mainMenu.SetActive(true);
            ES.SetSelectedGameObject(mainMenuButton);
            currentMenu = Menus.Main;
        }

        public void EnterControlsMenu()
        {
            optionsButton = ES.currentSelectedGameObject;
            controlsMenu.SetActive(true);
            optionsMenu.SetActive(false);
            ES.SetSelectedGameObject(null);
            currentMenu = Menus.Controls;
        }
        public void ExitcontrolsMenu()
        {
            controlsMenu.SetActive(false);
            optionsMenu.SetActive(true);
            ES.SetSelectedGameObject(optionsButton);
            currentMenu = Menus.Options;
        }
        public void EnterAudioMenu()
        {
            optionsButton = ES.currentSelectedGameObject;
            audioMenu.SetActive(true);
            optionsMenu.SetActive(false);
            ES.SetSelectedGameObject(audioButton);
            currentMenu = Menus.Audio;
        }
        public void ExitAudioMenu()
        {
            audioButton = ES.currentSelectedGameObject;
            audioMenu.SetActive(false);
            optionsMenu.SetActive(true);
            ES.SetSelectedGameObject(optionsButton);
            currentMenu = Menus.Options;
        }
        public void EnterCreditsMenu()
        {
            optionsButton = ES.currentSelectedGameObject;
            creditsMenu.SetActive(true);
            optionsMenu.SetActive(false);
            ES.SetSelectedGameObject(null);
            currentMenu = Menus.Credits;
        }
        public void ExitCreditsMenu()
        {
            creditsMenu.SetActive(false);
            optionsMenu.SetActive(true);
            ES.SetSelectedGameObject(optionsButton);
            currentMenu = Menus.Options;
        }
    }
}
