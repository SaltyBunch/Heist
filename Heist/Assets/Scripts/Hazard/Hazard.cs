using UnityEngine;

namespace Hazard
{
    [RequireComponent(typeof(Collider))]
    public class Hazard : MonoBehaviour
    {
        public virtual void Place(Vector3 position)
        {
        }

        public virtual void Trigger()
        {
        }

        public virtual void Defuse()
        {
        }

        public void Bind()
        {
            throw new System.NotImplementedException();
        }
    }
}