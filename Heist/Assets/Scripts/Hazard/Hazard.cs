using System;
using Character;
using UnityEngine;

namespace Hazard
{
    [RequireComponent(typeof(Collider))]
    public class Hazard : Game.Item, IEquatable<Hazard>
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

        public bool Equals(Hazard other)
        {
            return (this is ElectricField && other is ElectricField) || (this is LethalLaser && other is LethalLaser);
        }
    }
}