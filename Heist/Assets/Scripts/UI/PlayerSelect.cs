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

        [SerializeField] bool ready;

        // Update is called once per frame
        void Update()
        {
            if (ReInput.players.GetPlayer(player).GetButtonDown("UISubmit") && ready)
            {
                
                MenuManager.menuManager.RestartGame();
                mm.ExitPlayerSelect();
            }
            readyFX.SetActive(false);
            if (ReInput.players.GetPlayer(player).GetButtonDown("UIHorizontal") && !ready)
            {
                selection++;
            }
            if (ReInput.players.GetPlayer(player).GetButtonDown("UISubmit") && !ready)
            {
                ready = !ready;
            }
            
            if (!ready && ReInput.players.GetPlayer(player).GetButtonDown("UICancel"))
            {
                mm.ExitPlayerSelect();
            } 
                
            if (ready && ReInput.players.GetPlayer(player).GetButtonDown("UICancel"))
            {
                ready = !ready;
            }

            readyFX.SetActive(ready);

            switch (selection)
            {
                case 0:
                    display.text = charas[0];
                    break;
                case 1:
                    display.text = charas[1];
                    break;
                case 2:
                    display.text = charas[2];
                    break;
                case 3:
                    display.text = charas[3];
                    break;
                case 4:
                    selection = 0;
                    break;

            }
        }
    }
    
}
