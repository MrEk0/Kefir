using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using UnityEngine.Pool;

public class TargetFollowerObjectPoolCreator : ObjectPoolCreator, IGameFinishable
{
    [SerializeField] private AttackerPoolItem _poolItem;
    
    private Bounds _bounds;
    private GameUpdater _gameUpdater;
    private Transform _target;

    private readonly Dictionary<GameObject, ObjectTargetFollowerMovement> _objectMovements = new();

    public void Init(Bounds bounds, Transform target, ServiceLocator serviceLocator)
    {
        _bounds = bounds;
        _target = target;
        _gameUpdater = serviceLocator.GetService<GameUpdater>();
        
        ObjectPool = new ObjectPool<ObjectPoolItem>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);
    }
    
    protected override ObjectPoolItem CreateProjectile()
    {
        var spawnerItem = Instantiate(_poolItem, transform);

        var movement = new ObjectTargetFollowerMovement(spawnerItem.transform, _target, _bounds, _poolItem.Velocity, () =>
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

        if (pooledObject is AttackerPoolItem item)
            item.CollisionEvent -= OnCollision;
        
        Destroy(pooledObject.gameObject);
    }

    private void OnCollision(Collider2D other, AttackerPoolItem pooledObject)
    {
        var target = other.transform.GetComponent<IDamagable>();
        target?.TakeDamage();

        if (CanRelease(pooledObject))
            ObjectPool.Release(pooledObject);
    }

    public void GameFinish()
    {
        var keys = _objectMovements.Keys.ToList();
        for (var i = 0; i < keys.Count; i++)
        {
            _gameUpdater.RemoveListener(_objectMovements[keys[i]]);
            Destroy(keys[i]);
        }
        
        _objectMovements.Clear();
    }
}
