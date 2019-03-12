using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public class AnimControl : MonoBehaviour
    {
        [SerializeField] Animator animator;

        public void DoAttack()
        {
            animator.SetTrigger("shoot");
        }
        public void DoStun()
        {
            animator.SetTrigger("dead");
        }
        public void DoAlive()
        {
            animator.SetTrigger("alive");
        }
    }
}