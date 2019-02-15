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
        BigChase
    }

    public enum Command
    {
        Die,
        Wake,
        LosePlayer,
        SoundNotification,
        SeePlayer,
        LockDown
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
                {new StateTransition(State.Chase, Command.LosePlayer), State.Patrol},
                {new StateTransition(State.Investigate, Command.LosePlayer), State.Patrol},
                //Lock Down 
                {new StateTransition(State.Patrol, Command.LockDown), State.BigPatrol},
                {new StateTransition(State.Investigate, Command.LockDown), State.BigPatrol},
                {new StateTransition(State.Chase, Command.SeePlayer), State.BigPatrol},
                {new StateTransition(State.BigPatrol, Command.SeePlayer), State.BigChase},
                {new StateTransition(State.BigChase, Command.LosePlayer), State.BigPatrol}
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