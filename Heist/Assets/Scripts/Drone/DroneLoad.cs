using System.Collections;
using System.Collections.Generic;
using Character;
using Drone;
using UnityEngine;

namespace drone
{
    public class DroneLoad : MonoBehaviour
    {

        [SerializeField]public  List<Wave> droneWaves;
        // Start is called before the first frame update
        public void activateDrones()
        {
            foreach (var v in droneWaves)
            {
                StartCoroutine(delaySpawn(v));
            }


        }

        public void Begin(List<Player> playerList)
        {
            foreach (var wave in droneWaves)
            {
                foreach (var drone in wave.drones)
                {
                    drone.gameObject.SetActive(false);
                    
                    drone.SetPlayers(playerList);
                }
            }
            
            activateDrones();
        }


        IEnumerator delaySpawn(Wave w)
        {
            yield return new WaitForSeconds(w.spawnDelay);
            foreach(var drone in w.drones)
            {
                drone.gameObject.SetActive(true);
            }
        }

    }

    [System.Serializable]
    public class Wave
    {
        public float spawnDelay;
        [SerializeField] public List<DroneAI> drones;

    }
}

