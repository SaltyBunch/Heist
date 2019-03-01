using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{

    public class AnimationControl : MonoBehaviour
    {
        [SerializeField] Animator animator;
        [SerializeField] string runAnim;
        [SerializeField] string dashAnim;
        private readonly int m_speed;

        public void Run(float speed)
        {
            animator.Play(runAnim);
        }

        public void Dash()
        {
            animator.Play(dashAnim, 0, 0);
        }

    }
}