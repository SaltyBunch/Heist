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

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                var player = other.gameObject.GetComponentInParent<PlayerGameObject>();
                player.Camera.SwitchFloors(Floor);
                player.PlayerControl.Floor = Floor;
                LevelManager.LevelManagerRef.UpdateCameras();
            }
        }

        public void SetLayersRecursive(Transform floorTransform, Floor floor)
        {
            if (floorTransform.CompareTag("Hazard"))
            {
                var hazard = floorTransform.GetComponent<Hazard.Hazard>();
                if (hazard is LethalLaser laser)
                {
                    laser.SetFloor(LevelManager.HazardMask[floor]);
                }
                else if (hazard is ElectricField electric)
                {
                    electric.SetFloor(LevelManager.HazardMask[floor]);
                }
            }
            else if (floorTransform.CompareTag("Pickup"))
            {
                var pickup = floorTransform.GetComponent<Pickup.Pickup>();
                GameManager.SetLayerOnAll(pickup.gameObject, LevelManager.PickupMask[floor]);
            }
            else
            {
                floorTransform.gameObject.layer = LevelManager.EnvironementMask[floor];
                var children = floorTransform.childCount;
                for (int i = 0; i < children; i++)
                {
                    SetLayersRecursive(floorTransform.GetChild(i), floor);
                }
            }
        }
    }
}