using Game;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Drone : Character
    {
        [SerializeField] private NavMeshAgent agent;

        private new void Start()
        {
            if (agent == null)
                agent = GetComponent<NavMeshAgent>();
            base.Start();

            Stats = GameManager.CharacterStats[Characters.Drone];
        }
    }
}