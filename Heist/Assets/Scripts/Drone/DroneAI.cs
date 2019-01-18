using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Drone
{
    public class DroneAI : MonoBehaviour
    {
        public Transform Target;
        private NavMeshAgent agent;
        private FSM fsm;

        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            fsm = new FSM();
        }

        // Update is called once per frame
        void Update()
        {
            if (Target)
            {
                agent.destination = Target.position;
            }
            else
            {
                agent.destination = transform.position;
            }
            Debug.Log(fsm.CurrentState);

        }



        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                Target = other.transform;
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.tag == Target.gameObject.tag)
            {
                Target = null;
            }
        }
    }
}


