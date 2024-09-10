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
    public class TargetFollowerObjectPoolCreator : ObjectPoolCreator, IGameFinishable
    {
        [SerializeField] private AttackerPoolItem _poolItem;

        [CanBeNull] private GameUpdater _gameUpdater;
        [CanBeNull] private Transform _target;
        private Bounds _bounds;

        private readonly Dictionary<GameObject, ObjectTargetFollowerMovement> _objectMovements = new();

        public void Init(Bounds bounds, Transform target, ServiceLocator serviceLocator)
        {
            _bounds = bounds;
            _target = target;
            _gameUpdater = serviceLocator.GetService<GameUpdater>();

            ObjectPool = new ObjectPool<ObjectPoolItem>(CreateProjectile, OnGetFromPool, OnReleaseToPool,
                OnDestroyPooledObject);
        }

        protected override ObjectPoolItem CreateProjectile()
        {
            var spawnerItem = Instantiate(_poolItem, transform);

            var movement = new ObjectTargetFollowerMovement(spawnerItem.transform, _target, _bounds, _poolItem.Velocity,
                () =>
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

        protected override void OnDestroyPooledObject(ObjectPoolItem pooledObject)
        {
            if (_gameUpdater == null)
                return;

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
