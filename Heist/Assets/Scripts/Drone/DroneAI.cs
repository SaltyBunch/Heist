using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private List<GameObject> players => LevelManager.LevelManagerRef.Players.Select(x => x.PlayerControl.gameObject).ToList();
        [SerializeField] private float reviveTimer;
        [SerializeField] private bool reviving;
        public Transform Target;

        [SerializeField] private float atkRange;
        [SerializeField] private float atkSpeed;
        [SerializeField] private bool isShooter;
        private bool canAtk = true;
        [SerializeField] private AnimControl control;
        //[SerializeField] private ;

        [SerializeField] Character.Drone drone;


        // Start is called before the first frame update
        private void Start()
        {
            if (bigPatrolPath.Count <= 0) bigPatrolPath = patrolPath;
            investigation.transform.parent = null;
            foreach(var v in patrolPath)
            {
                v.parent = null;
            }
            if (bigPatrolPath.Count > 0)
            {
                foreach (var v in bigPatrolPath)
                {
                    if(v) v.parent = null;
                }
            }

            agent = GetComponent<NavMeshAgent>();
            obstacle = GetComponent<NavMeshObstacle>();
            drone = this.GetComponent<Character.Drone>();
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
            if (!reviving && drone.Stunned) fsm.MoveNext(Command.Die);

            if (!reviving && Input.GetKeyDown(KeyCode.Z) && Input.GetKeyDown(KeyCode.M)) fsm.MoveNext(Command.LockDown);

            //Patrol State
            if (fsm.CurrentState.Equals(State.Patrol))
            {
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 1f)
                    Target = patrolPath[patrol++ % patrolPath.Count];

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange && !v.GetComponentInChildren<Character.Player>().Stunned)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
                    
            }

            //Investigate State
            else if (fsm.CurrentState.Equals(State.Investigate))
            {
                //Arrive at investigate zone
                if (Vector3.Distance(transform.position, investigation.transform.position) < 1f)
                {
                    Target = patrolPath[patrol];
                    fsm.MoveNext(Command.LosePlayer);
                }

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange && !v.GetComponentInChildren<Character.Player>().Stunned)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }

            //Chase State
            else if (fsm.CurrentState.Equals(State.Chase))
            {
                if (!investg && Vector3.Distance(transform.position, lastLoc) > patrolTetherRange)
                {
                    investg = true;
                    StartCoroutine(DoInvestigate());
                }
                //shoot player
                foreach (var v in players)
                {
                    if (Vector3.Distance(transform.position, v.transform.position) < atkRange && canAtk)
                    {
                        gameObject.GetComponent<Weapon.StunGun>().Attack();
                        canAtk = false;
                        StartCoroutine(reload());
                    }
                    if (v.GetComponentInChildren<Character.Player>().Stunned)
                    {
                        fsm.MoveNext(Command.LosePlayer);
                    }
                }
            }

            //BigPatrol State
            else if (fsm.CurrentState.Equals(State.BigPatrol))
            {
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 0.2f)
                {
                    if (patrol >= patrolPath.Count) patrol = -1;

                    Target = bigPatrolPath[++patrol];
                }

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange && !v.GetComponentInChildren<Character.Player>().Stunned)
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }

            //BigChase State
            else if (fsm.CurrentState.Equals(State.BigChase))
            {
                //Do Big Chase
                //shoot player
                foreach (var v in players)
                {
                    if (Vector3.Distance(transform.position, v.transform.position) < atkRange && canAtk)
                    {
                        gameObject.GetComponent<Weapon.StunGun>().Attack();
                        canAtk = false;
                        StartCoroutine(reload());
                    }
                    if (v.GetComponentInChildren<Character.Player>().Stunned)
                    {
                        fsm.MoveNext(Command.LosePlayer);
                    }
                }
            }

            //Dead State
            else if (fsm.CurrentState.Equals(State.Dead))
            {
                control.DoStun();
                reviving = true;
                Target = null;
                if (!drone.Stunned)
                {
                    Target = patrolPath[0];
                    fsm.MoveNext(Command.Wake);
                    control.DoAlive();
                }
            }

            if (Target)
                agent.destination = Target.position;
            else
                agent.destination = transform.position;
        }


        private IEnumerator DoInvestigate()
        {
            yield return new WaitForSeconds(investigateDuration);
            Target = patrolPath[patrol];
            fsm.MoveNext(Command.LosePlayer);
            investg = false;
        }

        private IEnumerator reload()
        {
            yield return new WaitForSeconds(atkSpeed);
            canAtk = true;
        }

        private void OnTriggerEnter(Collider other)
        {
           // if (other.tag == "Player") players.Add(other.gameObject);
        }

        private void OnTriggerExit(Collider other)
        {
          //  if (other.tag == "Player") players.Remove(other.gameObject);
        }
    }
}