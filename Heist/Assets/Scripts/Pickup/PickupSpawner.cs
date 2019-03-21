using System.Collections.Generic;
using UnityEngine;

namespace Pickup
{
    public class PickupSpawner : MonoBehaviour
    {
        [SerializeField] private List<Pickup> _pickups;

        // Start is called before the first frame update
        public void Spawn()
        {
            var i = Random.Range(0, _pickups.Count);

            Instantiate(_pickups[i], transform.position, transform.rotation, null);

            Destroy(this.gameObject);
        }
    }
}