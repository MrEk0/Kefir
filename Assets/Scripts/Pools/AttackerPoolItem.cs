using System;
using UnityEngine;

namespace Pools
{
    [RequireComponent(typeof(Collider2D))]
    public class AttackerPoolItem : ObjectPoolItem
    {
        [SerializeField] private int _velocity;
        [SerializeField] private LayerMask _attackMask;

        protected LayerMask AttackMask => _attackMask;
        public int Velocity => _velocity;

        public event Action<Collider2D, AttackerPoolItem> CollisionEvent = delegate { };

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_attackMask.value != (_attackMask.value | (1 << other.gameObject.layer)))
                return;

            CollisionEvent(other, this);
        }
    }
}
