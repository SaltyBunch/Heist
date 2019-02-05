using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.AI;

namespace Drone
{
    public class DroneAI : MonoBehaviour
    {
        public Transform Target;
        private NavMeshAgent agent;
        private NavMeshObstacle obstacle;
        public FSM fsm;
        [SerializeField] List<Transform> patrolPath;
        [SerializeField] List<Transform> bigPatrolPath;
        [SerializeField] List<GameObject> players;
        [SerializeField] float detectPlayerRange;
        [SerializeField] float patrolTetherRange;
        [SerializeField] float investigateDuration;
        [SerializeField] GameObject investigation;
        [SerializeField] float reviveTimer;
        private Vector3 lastLoc;
        bool investg = false;
        [SerializeField] private bool reviving = false;
        int patrol = 0;


        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            obstacle = GetComponent<NavMeshObstacle>();
            if (bigPatrolPath.Count <= 0) bigPatrolPath = patrolPath;
            fsm = new FSM();
            Target = patrolPath[0];

            LevelManager.LevelManagerRef.Notifty += LevelManagerRefOnNotifty;
        }

        private void LevelManagerRefOnNotifty(object sender, NotifyEventArgs e)
        {
            //todo use notify type
            // e.NotifyType
            if (Vector3.Distance(transform.position, e.Position) <=
                LevelManager.LevelManagerRef.NotificationRamge[e.NotifyType])
            {
                //todo check layers
                fsm.MoveNext(Command.SoundNotification);
                investigation.transform.position = e.Position;
            }
        }

        // Update is called once per frame
        void Update()
        {
            //cheats
            if (!reviving && Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.P))
            {
                fsm.MoveNext(Command.Die);
            }

            if (!reviving && Input.GetKeyDown(KeyCode.Z) && Input.GetKeyDown(KeyCode.M))
            {
                fsm.MoveNext(Command.LockDown);
            }

            //Patrol State
            if (fsm.CurrentState.Equals(State.Patrol))
            {
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 1f)
                {
                    Target = patrolPath[patrol++ % patrolPath.Count];
                }

                //Detect player
                foreach (var v in players)
                {
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
                }

                //Detect Sound
//                foreach (var v in players)
//                {
//                    if (v.transform.parent.GetComponent<Character.PlayerControl>().Noise)
//                    {
//                        investigation.transform.position = v.transform.position;
//                        Target = investigation.transform;
//                        lastLoc = transform.position;
//                        fsm.MoveNext(Command.SoundNotification);
//                    }
//                }
            }

            //Investigate State
            if (fsm.CurrentState.Equals(State.Investigate))
            {
                //Arrive at investigate zone
                if (Vector3.Distance(transform.position, investigation.transform.position) < 0.5f)
                {
                    Target = patrolPath[patrol];
                    fsm.MoveNext(Command.LosePlayer);
                }

                //Detect player
                foreach (var v in players)
                {
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
                }
            }

            //Chase State
            if (fsm.CurrentState.Equals(State.Chase))
            {
                if (!investg && Vector3.Distance(transform.position, lastLoc) > patrolTetherRange)
                {
                    investg = true;
                    StartCoroutine(DoInvestigate());
                }
            }

            //BigPatrol State
            if (fsm.CurrentState.Equals(State.BigPatrol))
            {
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 0.2f)
                {
                    if (patrol >= patrolPath.Count)
                    {
                        patrol = -1;
                    }

                    Target = bigPatrolPath[++patrol];
                }

                //Detect player
                foreach (var v in players)
                {
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
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
            {
                agent.destination = Target.position;
            }
            else
            {
                agent.destination = transform.position;
            }

            Debug.Log(fsm.CurrentState);
        }

        IEnumerator Revive()
        {
            yield return new WaitForSeconds(reviveTimer);
            Target = patrolPath[patrol];
            fsm.MoveNext(Command.Wake);
            reviving = false;
        }

        IEnumerator DoInvestigate()
        {
            yield return new WaitForSeconds(investigateDuration);
            Target = patrolPath[patrol];
            fsm.MoveNext(Command.LosePlayer);
            investg = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                players.Add(other.gameObject);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.tag == "Player")
            {
                players.Remove(other.gameObject);
            }
        }
    }
}