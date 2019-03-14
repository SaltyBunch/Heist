using System.Collections;
using System.Collections.Generic;
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

        public void Begin()
        {
            foreach (var v in droneWaves)
            {
                foreach (var w in v.drones)
                {
                    w.gameObject.SetActive(false);
                }
            }
        }


        IEnumerator delaySpawn(Wave w)
        {
            yield return new WaitForSeconds(w.spawnDelay);
            foreach(var v in w.drones)
            {
                v.gameObject.SetActive(true);
            }
        }

    }

    [System.Serializable]
    public class Wave
    {
        public float spawnDelay;
        [SerializeField] public List<GameObject> drones;

    }
}

