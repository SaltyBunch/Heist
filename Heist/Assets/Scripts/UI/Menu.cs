using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.EventSystems;

namespace UI
{
    enum Menus
    {
        Options, Controls, Audio, Credits
    }

    public class Menu : MonoBehaviour
    {
        [SerializeField] EventSystem ES;

        [SerializeField] GameObject optionsMenu;
        [SerializeField] GameObject startButton;
        [SerializeField] GameObject controlsMenu;
        [SerializeField] GameObject audioMenu;
        [SerializeField] GameObject audioButton;
        [SerializeField] GameObject creditsMenu;

        private Menus currentMenu;

        // Start is called before the first frame update
        void Start()
        {

            LoadUIControls();
            currentMenu = Menus.Options;
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

        public void EnterControlsMenu()
        {
            startButton = ES.currentSelectedGameObject;
            controlsMenu.SetActive(true);
            optionsMenu.SetActive(false);
            ES.SetSelectedGameObject(null);
            currentMenu = Menus.Controls;
        }
        public void ExitcontrolsMenu()
        {
            controlsMenu.SetActive(false);
            optionsMenu.SetActive(true);
            ES.SetSelectedGameObject(startButton);
            currentMenu = Menus.Options;
        }
        public void EnterAudioMenu()
        {
            startButton = ES.currentSelectedGameObject;
            audioMenu.SetActive(true);
            optionsMenu.SetActive(false);
            ES.SetSelectedGameObject(audioButton);
            currentMenu = Menus.Audio;
        }
        public void ExitAudioMenu()
        {
            audioMenu.SetActive(false);
            optionsMenu.SetActive(true);
            ES.SetSelectedGameObject(startButton);
            currentMenu = Menus.Options;
        }
        public void EnterCreditsMenu()
        {
            startButton = ES.currentSelectedGameObject;
            creditsMenu.SetActive(true);
            optionsMenu.SetActive(false);
            ES.SetSelectedGameObject(null);
            currentMenu = Menus.Credits;
        }
        public void ExitCreditsMenu()
        {
            creditsMenu.SetActive(false);
            optionsMenu.SetActive(true);
            ES.SetSelectedGameObject(startButton);
            currentMenu = Menus.Options;
        }
    }
}
