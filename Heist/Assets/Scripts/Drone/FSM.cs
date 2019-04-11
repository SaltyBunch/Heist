using System;
using System.Collections.Generic;
using UnityEngine;

namespace Drone
{
    public enum State
    {
        Dead,
        Patrol,
        Investigate,
        Chase,
        BigPatrol,
        BigChase,
        ReturnToPatrol, 
        ReturnToBigPatrol, 
    }

    public enum Command
    {
        Die,
        Wake,
        LosePlayer,
        SoundNotification,
        SeePlayer,
        LockDown,
        AtPatrolPoint
    }


    public class FSM : MonoBehaviour
    {
        private Dictionary<StateTransition, State> transitions;

        public FSM()
        {
            Process();
        }

        public State CurrentState { get; private set; }

        public void Process()
        {
            CurrentState = State.Patrol;
            transitions = new Dictionary<StateTransition, State>
            {
                //Wake up from death
                {new StateTransition(State.Dead, Command.Wake), State.Patrol},
                {new StateTransition(State.Dead, Command.LosePlayer), State.Dead},
                //Die
                {new StateTransition(State.BigChase, Command.Die), State.Dead},
                {new StateTransition(State.BigPatrol, Command.Die), State.Dead},
                {new StateTransition(State.Chase, Command.Die), State.Dead},
                {new StateTransition(State.Investigate, Command.Die), State.Dead},
                {new StateTransition(State.Patrol, Command.Die), State.Dead},
                //notified of player
                {new StateTransition(State.Patrol, Command.SoundNotification), State.Investigate},
                //Chase player
                {new StateTransition(State.Patrol, Command.SeePlayer), State.Chase},
                {new StateTransition(State.Investigate, Command.SeePlayer), State.Chase},
                //Lose player
                {new StateTransition(State.Chase, Command.LosePlayer), State.ReturnToPatrol},
                {new StateTransition(State.Investigate, Command.LosePlayer), State.ReturnToPatrol},
                {new StateTransition(State.Patrol, Command.LosePlayer), State.ReturnToPatrol},
                //Return to patrol
                {new StateTransition(State.ReturnToPatrol, Command.AtPatrolPoint), State.Patrol},
                {new StateTransition(State.ReturnToPatrol, Command.LosePlayer), State.Patrol},
                {new StateTransition(State.ReturnToPatrol, Command.Die), State.Dead},
                {new StateTransition(State.ReturnToBigPatrol, Command.Die), State.Dead},
                {new StateTransition(State.ReturnToBigPatrol, Command.AtPatrolPoint), State.BigPatrol},
                //Lock Down 
                {new StateTransition(State.Patrol, Command.LockDown), State.BigPatrol},
                {new StateTransition(State.Investigate, Command.LockDown), State.ReturnToBigPatrol},
                {new StateTransition(State.BigPatrol, Command.SeePlayer), State.BigChase},
                {new StateTransition(State.BigChase, Command.LosePlayer), State.ReturnToBigPatrol}
            };
        }

        public State GetNext(Command command)
        {
            var transition = new StateTransition(CurrentState, command);
            State nextState;
            if (!transitions.TryGetValue(transition, out nextState))
                throw new Exception("Invalid transitions: " + CurrentState + " -> " + command);
            return nextState;
        }

        public State MoveNext(Command command)
        {
            CurrentState = GetNext(command);
            return CurrentState;
        }

        private class StateTransition
        {
            private readonly Command Command;
            private readonly State CurrentState;

            public StateTransition(State currentState, Command command)
            {
                CurrentState = currentState;
                Command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * CurrentState.GetHashCode() + 31 * Command.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                var other = obj as StateTransition;
                return other != null && CurrentState == other.CurrentState && Command == other.Command;
            }
        }
    }
}