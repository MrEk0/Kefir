using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;
using UnityEngine.Pool;

public class AttackerObjectPoolCreator : ObjectPoolCreator, IGameFinishable
{
    [SerializeField] private AttackerPoolItem _poolItem;
    
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
