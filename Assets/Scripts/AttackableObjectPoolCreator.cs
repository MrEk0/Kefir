using System.Collections.Generic;
using Interfaces;
using UnityEngine;
using UnityEngine.Pool;

public class AttackableObjectPoolCreator : ObjectPoolCreator
{
    [SerializeField] private AttackablePoolItem _poolItem;
    
    private Bounds _bounds;
    private GameUpdater _gameUpdater;

    private readonly Dictionary<GameObject, ObjectMovement> _objectMovements = new();

    public void Init(Bounds bounds, ServiceLocator serviceLocator)
    {
        _bounds = bounds;
        _gameUpdater = serviceLocator.GetService<GameUpdater>();
        
        ObjectPool = new ObjectPool<ObjectPoolItem>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);
    }
    
    protected override ObjectPoolItem CreateProjectile()
    {
        var spawnerItem = Instantiate(_poolItem, transform);

        var movement = new ObjectMovement(spawnerItem.transform, _bounds, _poolItem.Velocity, () =>
        {
            if (CanRelease(spawnerItem))
                ObjectPool.Release(spawnerItem);
        });

        _objectMovements.Add(spawnerItem.gameObject, movement);
        _gameUpdater.AddListener(movement);

        spawnerItem.CollisionEvent += OnCollision;

        return spawnerItem;
    }

    protected override void OnDestroyPooledObject(ObjectPoolItem pooledObject)
    {
        if (_objectMovements.TryGetValue(pooledObject.gameObject, out var movement))
        {
            _gameUpdater.RemoveListener(movement);
            _objectMovements.Remove(pooledObject.gameObject);
        }

        if (pooledObject is AttackablePoolItem item)
            item.CollisionEvent -= OnCollision;
        
        Destroy(pooledObject.gameObject);
    }

    private void OnCollision(Collider2D other, AttackablePoolItem pooledObject)
    {
        var target = other.transform.GetComponent<IDamagable>();
        target?.TakeDamage();

        if (CanRelease(pooledObject))
            ObjectPool.Release(pooledObject);
    }
}
