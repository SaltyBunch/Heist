using System.Collections;
using System.Collections.Generic;
using Character;
using Game;
using Hazard;
using UnityEngine;
using UnityEngine.AI;
using Weapon;

namespace Drone
{
    public class DroneAI : MonoBehaviour
    {
        [SerializeField] private DroneState _droneState;
        [SerializeField] private ElectricField _eField;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private List<MeshRenderer> _meshes;

        [SerializeField] private AudioClip _patrolClip, _investigationClip, _attackClip, _damageClip, _stunClip;
        private MaterialPropertyBlock _prop;

        [SerializeField] private StunGun _stunGun;

        [Header("Texture")] [SerializeField] private List<Texture2D> _textures;
        private NavMeshAgent agent;

        [SerializeField] private float atkRange;
        [SerializeField] private float atkSpeed;
        [SerializeField] private List<Transform> bigPatrolPath;
        private bool canAtk = true;
        private bool canMove = true;

        [SerializeField] private Animator control;

        [SerializeField] private float detectPlayerRange;
        //[SerializeField] private ;

        [SerializeField] private Character.Drone drone;
        public FSM fsm;
        private bool investg;
        [SerializeField] private float investigateDuration;
        [SerializeField] private GameObject investigation;
        [SerializeField] private bool isShooter;
        private Vector3 lastLoc;
        private NavMeshObstacle obstacle;
        private int patrol;
        [SerializeField] private List<Transform> patrolPath;
        [SerializeField] private float patrolTetherRange;
        [SerializeField] private List<Player> players;
        [SerializeField] private float reviveTimer;
        [SerializeField] private bool reviving;
        public Transform Target;

        // Start is called before the first frame update
        private void Start()
        {
            if (bigPatrolPath.Count <= 0) bigPatrolPath = patrolPath;
            investigation.transform.parent = null;
            foreach (var v in patrolPath) v.parent = null;

            if (bigPatrolPath.Count > 0)
                foreach (var v in bigPatrolPath)
                    if (v)
                        v.parent = null;

            agent = GetComponent<NavMeshAgent>();
            obstacle = GetComponent<NavMeshObstacle>();
            drone = GetComponent<Character.Drone>();
            Target = patrolPath[0];

            agent.speed = 3;

            //agent.speed = drone.Stats.Speed;
            agent.Warp(transform.position);

            _prop = new MaterialPropertyBlock();

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

        public void SetPlayers(List<Player> playerList)
        {
            players = playerList;
        }

        // Update is called once per frame
        private void Update()
        {
            //cheats
            if (!reviving && drone.Stunned) fsm.MoveNext(Command.Die);

            if (!reviving && Input.GetKeyDown(KeyCode.Z) && Input.GetKeyDown(KeyCode.M)) fsm.MoveNext(Command.LockDown);

            canMove = true;
            //Patrol State
            if (fsm.CurrentState.Equals(State.Patrol))
            {
                _droneState = DroneState.Patrol;
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 1f)
                    Target = patrolPath[++patrol % patrolPath.Count];

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange && !v.Stunned &&
                        FieldOFView(v, 60))
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }
            else if (fsm.CurrentState.Equals(State.ReturnToPatrol))
            {
                _droneState = DroneState.Patrol;
                if (Vector3.Distance(transform.position, Target.position) < 1f)
                    fsm.MoveNext(Command.AtPatrolPoint);
            }

            //Investigate State
            else if (fsm.CurrentState.Equals(State.Investigate))
            {
                _droneState = DroneState.Caution;
                //Arrive at investigate zone
                if (Vector3.Distance(transform.position, investigation.transform.position) < 1f)
                {
                    Target = patrolPath[patrol %= patrolPath.Count];
                    fsm.MoveNext(Command.LosePlayer);
                }

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange && !v.Stunned &&
                        FieldOFView(v, 60))
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }

            //Chase State
            else if (fsm.CurrentState.Equals(State.Chase))
            {
                _droneState = DroneState.Attack;

                if (!investg && Vector3.Distance(transform.position, lastLoc) > patrolTetherRange)
                {
                    investg = true;
                    StartCoroutine(DoInvestigate());
                }

                //shoot player
                foreach (var v in players)
                {
                    if (isShooter)
                    {
                        if (Vector3.Distance(transform.position, v.transform.position) < atkRange && canAtk &&
                            FieldOFView(v, 20))
                        {
                            _stunGun.Attack();
                            canAtk = false;
                            StartCoroutine(Reload());
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, v.transform.position) < atkRange && canAtk &&
                            FieldOFView(v, 180))
                        {
                            //TODO melee atk
                            canAtk = false;
                            StartCoroutine(Reload());
                        }
                    }

                    if (v.Stunned)
                    {
                        Target = patrolPath[patrol %= patrolPath.Count];
                        fsm.MoveNext(Command.LosePlayer);
                    }
                }
            }

            //BigPatrol State
            else if (fsm.CurrentState.Equals(State.BigPatrol))
            {
                _droneState = DroneState.Caution;
                //chaing patrol dest
                if (Vector3.Distance(transform.position, Target.position) < 0.2f)
                {
                    if (patrol >= patrolPath.Count) patrol = -1;

                    Target = bigPatrolPath[++patrol];
                }

                //Detect player
                foreach (var v in players)
                    if (Vector3.Distance(transform.position, v.transform.position) < detectPlayerRange && !v.Stunned &&
                        FieldOFView(v, 60))
                    {
                        Target = v.transform;
                        lastLoc = transform.position;
                        fsm.MoveNext(Command.SeePlayer);
                    }
            }
            else if (fsm.CurrentState.Equals(State.ReturnToBigPatrol))
            {
                _droneState = DroneState.Patrol;
                if (Vector3.Distance(transform.position, Target.position) < 1f)
                    fsm.MoveNext(Command.AtPatrolPoint);
            }
            //BigChase State
            else if (fsm.CurrentState.Equals(State.BigChase))
            {
                _droneState = DroneState.Attack;
                //Do Big Chase
                //shoot player
                foreach (var v in players)
                {
                    if (Vector3.Distance(transform.position, v.transform.position) < atkRange && canAtk &&
                        FieldOFView(v, 20))
                    {
                        if (isShooter) _stunGun.Attack();
                        control.SetTrigger("shoot");

                        canAtk = false;
                        StartCoroutine(Reload());
                    }

                    if (v.Stunned)
                    {
                        Target = patrolPath[patrol %= patrolPath.Count];
                        fsm.MoveNext(Command.LosePlayer);
                    }
                }
            }

            //Dead State
            else if (fsm.CurrentState.Equals(State.Dead))
            {
                _droneState = DroneState.Patrol;

                control.SetTrigger("dead");
                reviving = true;
                canMove = false;
                if (!drone.Stunned)
                {
                    Target = patrolPath[patrol %= patrolPath.Count];
                    fsm.MoveNext(Command.Wake);
                    control.SetTrigger("alive");
                }
            }

            if (canMove)
                agent.SetDestination(Target.position);
            //agent.destination = Target.position;
            else agent.SetDestination(transform.position);
            //agent.destination = transform.position;

            foreach (var character in _meshes)
            {
                character.GetPropertyBlock(_prop);
                _prop.SetTexture("_MainTex", _textures[(int) _droneState]);
                character.SetPropertyBlock(_prop);
            }
        }


        private IEnumerator DoInvestigate()
        {
            yield return new WaitForSeconds(investigateDuration);
            Target = patrolPath[patrol %= patrolPath.Count];
            fsm.MoveNext(Command.LosePlayer);
            investg = false;
        }

        private IEnumerator Reload()
        {
            yield return new WaitForSeconds(atkSpeed);
            canAtk = true;
        }

        private bool FieldOFView(Player player, float angle)
        {
            var dir = player.transform.position + Vector3.up - transform.position;
            if (Mathf.Abs(Vector3.Angle(transform.forward, dir)) < angle)
            {
                RaycastHit hit;
                Physics.Raycast(transform.position, dir, out hit, 30, _layerMask);
                return hit.transform.CompareTag("Player");
            }

            return false;
        }

        private enum DroneState
        {
            Patrol,
            Caution,
            Attack
        }
    }
}