using System;
using System.Collections;
using System.Collections.Generic;
using Character;
using Drone;
using UnityEngine;

namespace drone
{
    public class DroneLoad : MonoBehaviour
    {
        [SerializeField] public List<Wave> droneWaves;

        // Start is called before the first frame update
        public void activateDrones()
        {
            foreach (var v in droneWaves) StartCoroutine(delaySpawn(v));
        }

        public void Begin(List<Player> playerList)
        {
            foreach (var wave in droneWaves)
            foreach (var drone in wave.drones)
            {
                drone.gameObject.SetActive(false);

                drone.SetPlayers(playerList);
            }

            activateDrones();
        }


        private IEnumerator delaySpawn(Wave w)
        {
            yield return new WaitForSeconds(w.spawnDelay);
            foreach (var drone in w.drones) drone.gameObject.SetActive(true);
        }
    }

    [Serializable]
    public class Wave
    {
        [SerializeField] public List<DroneAI> drones;
        public float spawnDelay;
    }
}