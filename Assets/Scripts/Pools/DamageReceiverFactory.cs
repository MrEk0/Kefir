using System.Collections.Generic;
using System.Linq;
using Common;
using Game;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Pool;

namespace Pools
{
    public class DamageReceiverFactory : ObjectPoolFactory, IGameFinishListener
    {
        [SerializeField] private DamageReceiverPoolItem _poolItem;

        [CanBeNull] private GameUpdater _gameUpdater;
        private Bounds _bounds;

        private readonly Dictionary<GameObject, ObjectMovement> _objectMovements = new();
        
        public IObjectPool<DamageReceiverPoolItem> ObjectPool { get; private set; }

        public void Init(Bounds bounds, ServiceLocator serviceLocator)
        {
            _bounds = bounds;
            _gameUpdater = serviceLocator.GetService<GameUpdater>();

            ObjectPool = new ObjectPool<DamageReceiverPoolItem>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject);
        }

        private DamageReceiverPoolItem CreateProjectile()
        {
            var spawnerItem = Instantiate(_poolItem, transform);

            var movement = new ObjectMovement(spawnerItem.transform, _bounds, _poolItem.Velocity, () =>
            {
                if (CanRelease(spawnerItem))
                    ObjectPool.Release(spawnerItem);
            });

            if (_gameUpdater == null)
                return spawnerItem;

            _objectMovements.Add(spawnerItem.gameObject, movement);
            _gameUpdater.AddListener(movement);

            spawnerItem.CollisionEvent += OnCollision;

            return spawnerItem;
        }

        private void OnDestroyPooledObject(DamageReceiverPoolItem pooledObject)
        {
            if (_gameUpdater == null)
                return;

            if (_objectMovements.TryGetValue(pooledObject.gameObject, out var movement))
            {
                _gameUpdater.RemoveListener(movement);
                _objectMovements.Remove(pooledObject.gameObject);
            }
            
            pooledObject.CollisionEvent -= OnCollision;

            Destroy(pooledObject.gameObject);
        }

        private void OnCollision(Collider2D other, DamageReceiverPoolItem pooledObject)
        {
            var target = other.transform.GetComponent<IDamagable>();
            target?.TakeDamage();

            if (CanRelease(pooledObject))
                ObjectPool.Release(pooledObject);
        }

        public void GameFinish()
        {
            if (_gameUpdater == null)
                return;

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
