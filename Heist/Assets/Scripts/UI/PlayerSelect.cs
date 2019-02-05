using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEngine.UI;

namespace UI
{

    public class PlayerSelect : MonoBehaviour
    {
        [SerializeField] Menu mm;
        [SerializeField] int player;
        [SerializeField] Text display;
        [SerializeField] GameObject readyFX;
        [SerializeField] int selection;
        [SerializeField] string[] charas = { "King", "Shadow", "Jailbird", "Racoon" };
        private bool _delay = true;

        public bool ready = false;
        private void OnEnable()
        {
            if (ReInput.controllers.joystickCount <= player) gameObject.SetActive(false);
            else gameObject.SetActive(true);

        }

        // Update is called once per frame
        void Update()
        {
            readyFX.SetActive(false);
            if (ReInput.players.GetPlayer(player).GetButtonDown("UIHorizontaPos") && !ready)
            {
                selection++;
            }
            if (ReInput.players.GetPlayer(player).GetButtonDown("UIHorizontaNeg") && !ready)
            {
                selection--;
            }

            if (ReInput.players.GetPlayer(player).GetButtonDown("UISubmit") && !ready)
            {
                ready = true;
            }

            if (!ready && ReInput.players.GetPlayer(player).GetButtonDown("UICancel"))
            {
                mm.ExitPlayerSelect();
            }

            if (ready && ReInput.players.GetPlayer(player).GetButtonDown("UICancel"))
            {
                ready = false;
            }

            readyFX.SetActive(ready);

            switch (selection % 4)
            {

                case -1:
                    selection = 3;
                    break;
                case 4:
                    selection = 0;
                    break;
                case 0:
                    display.text = charas[0];
                    Game.GameManager.PlayerChoice[player] = Game.Characters.King;
                    break;
                case 1:
                    display.text = charas[1];
                    Game.GameManager.PlayerChoice[player] = Game.Characters.Shadow;
                    break;
                case 2:
                    display.text = charas[2];
                    Game.GameManager.PlayerChoice[player] = Game.Characters.Jailbird;
                    break;
                case 3:
                    display.text = charas[3];
                    Game.GameManager.PlayerChoice[player] = Game.Characters.Raccoon;
                    break;


            }
        }
    }

}
