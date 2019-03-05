using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Camera;
using Character;
using Rewired;
using Player = Rewired.Player;

namespace Controller
{

    public class AnimationControl : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] string runAnim;
        [SerializeField] string dashAnim;
        [SerializeField] string idleAnim;
        [SerializeField] private PlayerControl _playerControl;

        public Player Player => _playerControl.Player;
        private bool isRunning = false;

        public void Update()
        {
            float move = Mathf.Abs( Player.GetAxis("Move Vertical")) + Mathf.Abs(Player.GetAxis("Move Horizontal"));


            if (!isRunning && move != 0)
            {
                animator.CrossFadeInFixedTime(runAnim, 0.1f);
                isRunning = true;
            }

            if (isRunning && move == 0)
            {
                animator.CrossFadeInFixedTime(idleAnim, 0.3f);
                isRunning = false;
            }

            if (Player.GetButton("Dash"))
            {
                animator.CrossFadeInFixedTime(dashAnim, 0.1f);
            }
        }

    }
}