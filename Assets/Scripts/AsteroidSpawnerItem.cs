using System;
using UnityEngine;

public class AsteroidSpawnerItem : ObjectSpawnerItem
{
    [SerializeField] private LayerMask _playerBulletMask;

    public event Action<Vector3> PlayerHitEvent = delegate { };

    protected void OnCollisionEnter(Collision other)
    {
        if (_playerBulletMask == (_playerBulletMask | (1 << other.gameObject.layer)))
        {
            PlayerHitEvent(transform.position);
        }
    }
}
