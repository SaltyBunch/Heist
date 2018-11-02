using UnityEngine;

namespace Pickup
{
    [RequireComponent(typeof(Collider))]
    public class HazardPickup : MonoBehaviour
    {
        [SerializeField] private Hazard.Hazard _hazardGameObject;

        private void Start()
        {
            var collider = GetComponent<Collider>();
            if (!collider.isTrigger)
            {
                collider.isTrigger = true;
                Debug.LogError("Collider on {this.name} is not set as a Trigger, temporarily set to Trigger");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //pickup
            }
        }
    }
}