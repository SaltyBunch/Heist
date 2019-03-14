using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character {
    public class PlayerFOG : MonoBehaviour
    {
        public Transform FogOfWarPlane;
        public int num;

        // Update is called once per frame
        void Update()
        {
            if (num == 1) FogOfWarPlane.GetComponent<Renderer>().material.SetVector("_player1_Pos", transform.position);

            
        }
    }
}