using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.AI;

namespace Drone
{
    public class DroneAI : MonoBehaviour
    {
        private NavMeshAgent agent;
        [SerializeField] private List<Transform> bigPatrolPath;
        [SerializeField] private float detectPlayerRange;
        public FSM fsm;
        private bool investg;
        [SerializeField] private float investigateDuration;
        [SerializeField] private GameObject investigation;
        private Vector3 lastLoc;
        private NavMeshObstacle obstacle;
        private int patrol;
        [SerializeField] private List<Transform> patrolPath;
        [SerializeField] private float patrolTetherRange;
        [SerializeField] private List<GameObject> players;
        [SerializeField] private float reviveTimer;
        [SerializeField] private bool reviving;
        public Transform Target;


        // Start is called before the first frame update
        private void Start()
        {
            investigation.transform.parent = null;
            agent = GetComponent<NavMeshAgent>();
            obstacle = GetComponent<NavMeshObstacle>();
            if (bigPatrolPath.Count <= 0) bigPatrolPath = patrolPath;
            fsm = new FSM();
            Target = patrolPath[0];

            LevelManager.LevelManagerRef.Notifty += LevelManagerRefOnNotifty;

            foreach (var v in patrolPath)
            {
                v.transform.parent = null;
            }
            foreach (var v in bigPatrolPath)
            {
                v.transform.parent = null;
            }
        }

        private void LevelManagerRefOnNotifty(object sender, NotifyEventArgs e)
        {
            //todo use notify type
            // e.NotifyType
            if (Vector3.Distance(transform.position, e.Position) <=
                LevelManager.LevelManagerRef.NotificationRamge[e.NotifyType])
                if (fsm.CurrentState.Equals(State.Patrol))
                {
                    investigation.transform.position = e.Position;
                    if (Vector3.Distance(transform.position, investigation.transform.position) < 100)
                        fsm.MoveNext(Command.SoundNotification);
                    investigation.transform.position = e.Position;
                    Target = investigation.transform;
                }
        }

        // Update is called once per frame
        private void Update()
        {
            //cheats
            if (!reviving && Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.P)) fsm.MoveNext(Command.Die);

            if (!reviving && Input.GetKeyDown(KeyCode.Z) && Input.GetKeyDown(KeyCode.M)) fsm.MoveNext(Command.LockDown);

            //Patrol State
            if (fsm.CurrentState.Equals(State.Patrol))
            {
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 1f)
                    Target = patrolPath[patrol++ % patrolPath.Count];

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }

            //Investigate State
            if (fsm.CurrentState.Equals(State.Investigate))
            {
                //Arrive at investigate zone
                if (Vector3.Distance(transform.position, investigation.transform.position) < 1f)
                {
                    Target = patrolPath[patrol];
                    fsm.MoveNext(Command.LosePlayer);
                }

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }

            //Chase State
            if (fsm.CurrentState.Equals(State.Chase))
                if (!investg && Vector3.Distance(transform.position, lastLoc) > patrolTetherRange)
                {
                    investg = true;
                    StartCoroutine(DoInvestigate());
                }

            //BigPatrol State
            if (fsm.CurrentState.Equals(State.BigPatrol))
            {
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 0.2f)
                {
                    if (patrol >= patrolPath.Count) patrol = -1;

                    Target = bigPatrolPath[++patrol];
                }

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }

            //BigChase State
            if (fsm.CurrentState.Equals(State.BigChase))
            {
                //Do Big Chase
            }

            //Dead State
            if (fsm.CurrentState.Equals(State.Dead))
            {
                Target = null;
                if (!reviving)
                {
                    reviving = true;
                    StartCoroutine(Revive());
                }
            }

            if (Target)
                agent.destination = Target.position;
            else
                agent.destination = transform.position;
        }

        private IEnumerator Revive()
        {
            yield return new WaitForSeconds(reviveTimer);
            Target = patrolPath[patrol];
            fsm.MoveNext(Command.Wake);
            reviving = false;
        }

        private IEnumerator DoInvestigate()
        {
            yield return new WaitForSeconds(investigateDuration);
            Target = patrolPath[patrol];
            fsm.MoveNext(Command.LosePlayer);
            investg = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player") players.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player") players.Remove(other.gameObject);
        }
    }
}