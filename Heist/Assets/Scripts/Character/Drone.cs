using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Drone : Character
    {
        [SerializeField] NavMeshAgent agent;

        void Start()
        {
            if (agent == null)
                agent = GetComponent<NavMeshAgent>();
        }

        internal override void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
                {
                    agent.destination = hit.point;
                }
            }
        }
    }
}