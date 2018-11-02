using UnityEngine;

namespace Hazard
{
    [RequireComponent(typeof(Collider))]
    public class Hazard : MonoBehaviour
    {
        public virtual void Place()
        {
        }

        public virtual void Trigger()
        {
        }

        public virtual void Defuse()
        {
        }
    }
}