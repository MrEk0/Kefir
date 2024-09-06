using System;
using Interfaces;
using UnityEngine;

public class AsteroidPoolItem : ObjectPoolItem, IDamagable
{
    public event Action<Vector3> TakeDamageEvent = delegate { };
    
    public void TakeDamage()
    {
        TakeDamageEvent(transform.position);
        ObjectPool.Release(this);
    }
}
