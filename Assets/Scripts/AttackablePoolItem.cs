using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class AttackablePoolItem : ObjectPoolItem
{
    [SerializeField] private int _velocity;
    [SerializeField] private LayerMask _attackMask;
    
    public int Velocity => _velocity;
    
    public event Action<Collider2D, AttackablePoolItem> CollisionEvent = delegate { };
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_attackMask.value != (_attackMask.value | (1 << other.gameObject.layer))) 
            return;

        CollisionEvent(other, this);
    }
}
