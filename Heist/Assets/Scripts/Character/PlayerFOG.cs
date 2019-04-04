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
            if (num == 2) FogOfWarPlane.GetComponent<Renderer>().material.SetVector("_player2_Pos", transform.position);
            if (num == 3) FogOfWarPlane.GetComponent<Renderer>().material.SetVector("_player3_Pos", transform.position);
            if (num == 4) FogOfWarPlane.GetComponent<Renderer>().material.SetVector("_player4_Pos", transform.position);


        }
    }
}