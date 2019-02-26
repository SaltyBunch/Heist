using Drone;
using UnityEngine;
using UnityEngine.UI;

public class setState : MonoBehaviour
{
    public DroneAI drone;
    public Text text;


    // Update is called once per frame
    private void Update()
    {
        if (drone.fsm.CurrentState.Equals(State.BigChase)) text.text = "Current State : Lock Down Chase";
        if (drone.fsm.CurrentState.Equals(State.BigPatrol)) text.text = "Current State : Lock Down Patrol";
        if (drone.fsm.CurrentState.Equals(State.Chase)) text.text = "Current State : Chase";
        if (drone.fsm.CurrentState.Equals(State.Dead)) text.text = "Current State : Stunned";
        if (drone.fsm.CurrentState.Equals(State.Investigate)) text.text = "Current State : Investigating";
        if (drone.fsm.CurrentState.Equals(State.Patrol)) text.text = "Current State : Patrolling";
    }
}