using Character;
using UnityEngine;

namespace Hazard
{
    [RequireComponent(typeof(Collider))]
    public class Hazard : MonoBehaviour
    {
        public virtual void Place(Vector3 position)
        {
        }

        public virtual void Trigger(PlayerControl player)
        {
        }

        public virtual void Defuse()
        {
        }

        public void Bind(GameObject player)
        {
            throw new System.NotImplementedException();
        }
    }
}