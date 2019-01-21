using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setState : MonoBehaviour
{
    public Text text;
    public Drone.DroneAI drone;


    // Update is called once per frame
    void Update()
    {
        if (drone.fsm.CurrentState.Equals(Drone.State.BigChase))
        {
            text.text = "Current State : Lock Down Chase";
        }
        if (drone.fsm.CurrentState.Equals(Drone.State.BigPatrol))
        {
            text.text = "Current State : Lock Down Patrol";
        }
        if (drone.fsm.CurrentState.Equals(Drone.State.Chase))
        {
            text.text = "Current State : Chase";
        }
        if (drone.fsm.CurrentState.Equals(Drone.State.Dead))
        {
            text.text = "Current State : Stunned";
        }
        if (drone.fsm.CurrentState.Equals(Drone.State.Investigate))
        {
            text.text = "Current State : Investigating";
        }
        if (drone.fsm.CurrentState.Equals(Drone.State.Patrol))
        {
            text.text = "Current State : Patrolling";
        }

    }
}
