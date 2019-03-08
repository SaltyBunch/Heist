using Game;
using Rewired;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerSelect : MonoBehaviour
    {
        private bool _delay = true;
        [SerializeField] private string[] charas = {"King", "Shadow", "Jailbird", "Racoon"};
        [SerializeField] private Text display;
        [SerializeField] private Menu mm;
        [SerializeField] private int player;

        public bool ready;
        [SerializeField] private GameObject readyFX;
        [SerializeField] private int selection;

        private void OnEnable()
        {
            if (ReInput.controllers.joystickCount <= player) gameObject.SetActive(false);
            else gameObject.SetActive(true);
        }

        // Update is called once per frame
        private void Update()
        {
            
            
            
            readyFX.SetActive(false);
            if (ReInput.players.GetPlayer(player).GetAxisDelta("UIHorizontal") > 0.5f && ReInput.players.GetPlayer(player).GetAxis("UIHorizontal") > 0.5f && !ready) selection++;
            if (ReInput.players.GetPlayer(player).GetAxisDelta("UIHorizontal") < -0.5f && ReInput.players.GetPlayer(player).GetAxis("UIHorizontal") < -0.5f && !ready) selection--;

            if (ReInput.players.GetPlayer(player).GetButtonDown("UISubmit") && !ready) ready = true;

            if (!ready && ReInput.players.GetPlayer(player).GetButtonDown("UICancel")) mm.ExitPlayerSelect();

            if (ready && ReInput.players.GetPlayer(player).GetButtonDown("UICancel")) ready = false;

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
                    GameManager.PlayerChoice[player] = Characters.King;
                    break;
                case 1:
                    display.text = charas[1];
                    GameManager.PlayerChoice[player] = Characters.Shadow;
                    break;
                case 2:
                    display.text = charas[2];
                    GameManager.PlayerChoice[player] = Characters.Jailbird;
                    break;
                case 3:
                    display.text = charas[3];
                    GameManager.PlayerChoice[player] = Characters.Raccoon;
                    break;
            }
        }
    }
}