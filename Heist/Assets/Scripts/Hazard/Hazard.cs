using System;
using System.Collections;
using Game;
using UnityEngine;

namespace Hazard
{
    [RequireComponent(typeof(Collider))]
    public class Hazard : Item, IEquatable<Hazard>
    {
        public bool PlacedByPlayer;
        [SerializeField, Range(1, 25)] internal float _maxGap;
        [SerializeField] internal int Damage;

        internal Character.Character _placedBy;

        public bool Equals(Hazard other)
        {
            return this is ElectricField && other is ElectricField || this is LethalLaser && other is LethalLaser;
        }

        public virtual extern bool Place(Vector3 position, Character.Character player);

        internal IEnumerator RemovePlayer()
        {
            yield return new WaitForSeconds(1);
            _placedBy = null;
        }

        private void OnEnable()
        {
            StartCoroutine(RemovePlayer());
        }
    }
}