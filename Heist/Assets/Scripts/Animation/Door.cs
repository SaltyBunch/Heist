using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Animation
{

    public class Door : MonoBehaviour
    {
        [SerializeField]
        string openName;
        [SerializeField]
        string closeName;
        [SerializeField]
        Animator animator;
        //v2
        [SerializeField]
        string openName2;
        [SerializeField]
        string closeName2;
        [SerializeField]
        Animator animator2;

        [SerializeField]
        bool canOpen = true;

        private void Start()
        {
            animator.speed = 1.5f;
            if (animator2) animator2.speed = 1.5f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && canOpen)
            {
                animator.Play(openName,0,0);
                if (animator2) animator2.Play(openName2, 0, 0);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                animator.Play(closeName,0,0);
                if (animator2) animator2.Play(closeName2, 0, 0);
            }
        }

    }
}
