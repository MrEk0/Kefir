using System;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

namespace Pools
{
    public class DamageReceiverPoolItem : AttackerPoolItem, IDamagable
    {
        [CanBeNull] private Action<DamageReceiverPoolItem> _takeDamageAction;
        
        public new event Action<Collider2D, DamageReceiverPoolItem> CollisionEvent = delegate { };

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (AttackMask.value != (AttackMask.value | (1 << other.gameObject.layer)))
                return;

            CollisionEvent(other, this);
        }

        public void Init(Action<DamageReceiverPoolItem> action)
        {
            _takeDamageAction = action;
        }

        public void TakeDamage()
        {
            _takeDamageAction?.Invoke(this);
        }
    }
}
