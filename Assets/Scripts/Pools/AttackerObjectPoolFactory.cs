using System.Collections.Generic;
using System.Linq;
using Common;
using Game;
using Interfaces;
using UnityEngine;
using UnityEngine.Pool;

namespace Pools
{
    public class AttackerObjectPoolFactory : ObjectPoolFactory, IGameFinishListener
    {
        [SerializeField] private AttackerPoolItem _poolItem;

        private GameUpdater _gameUpdater;
        private Bounds _bounds;

        private readonly Dictionary<GameObject, ObjectMovement> _objectMovements = new();
        
        public IObjectPool<AttackerPoolItem> ObjectPool { get; private set; } 

        public void Init(Bounds bounds, ServiceLocator serviceLocator)
        {
            _bounds = bounds;
            _gameUpdater = serviceLocator.GetService<GameUpdater>();

            ObjectPool = new ObjectPool<AttackerPoolItem>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);
        }

        private AttackerPoolItem CreateProjectile()
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

        private void OnDestroyPooledObject(AttackerPoolItem pooledObject)
        {
            if (_objectMovements.TryGetValue(pooledObject.gameObject, out var movement))
            {
                _gameUpdater.RemoveListener(movement);
                _objectMovements.Remove(pooledObject.gameObject);
            }
            
            pooledObject.CollisionEvent -= OnCollision;

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
}
