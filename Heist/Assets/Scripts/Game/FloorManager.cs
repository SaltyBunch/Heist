using Character;
using Hazard;
using UnityEngine;

namespace Game
{
    public enum Floor
    {
        MainFloor,
        Basement
    }

    public class FloorManager : MonoBehaviour
    {
        [SerializeField] public Floor Floor;

        [SerializeField] public LayerMask[] Layers;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                var player = other.gameObject.GetComponentInParent<PlayerGameObject>();
                player.Camera.SwitchFloors(Floor);
            }
        }

        public void SetLayersRecursive(Transform floorTransform, Floor floor)
        {
            if (floorTransform.CompareTag("Hazard"))
            {
                var hazard = floorTransform.GetComponent<Hazard.Hazard>();
                if (hazard is LethalLaser laser)
                {
                    laser.SetFloor(floor, Layers[1]);
                }
                else if (hazard is ElectricField electric)
                {
                    electric.SetFloor(floor, Layers[1]);
                }
            }
            else if (floorTransform.CompareTag("Pickup"))
            {
                var pickup = floorTransform.GetComponent<Pickup.Pickup>();
                //todo pickup
            }
            else
            {
                floorTransform.gameObject.layer = Layers[0];
                var children = floorTransform.childCount;
                for (int i = 0; i < children; i++)
                {
                    SetLayersRecursive(floorTransform.GetChild(i), floor);
                }
            }
        }
    }
}