using System;
using Configs;
using Game;
using Interfaces;
using JetBrains.Annotations;
using Pools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawners
{
    public class UFOSpawner : IGameUpdatable, IServisable
    {
        public event Action UFOKilledEvent = delegate { };

        [CanBeNull] private readonly TargetFollowerObjectPoolCreator _objectPoolCreator;

        private readonly float _pause;

        private float _timer;
        private Bounds _bounds;

        public UFOSpawner(ServiceLocator serviceLocator, TargetFollowerObjectPoolCreator objectPoolCreator,
            Bounds bounds)
        {
            _objectPoolCreator = objectPoolCreator;
            _bounds = bounds;

            var data = serviceLocator.GetService<GameSettingsData>();

            _pause = data.UFOSpawnPause;
        }

        public void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer < _pause)
                return;

            SpawnNewUFO();

            _timer = 0f;
        }

        private void SpawnNewUFO()
        {
            if (_objectPoolCreator == null)
                return;

            var position = new Vector3(Random.Range(_bounds.min.x, _bounds.max.x), _bounds.max.y, 0f);

            var newPoolItem = _objectPoolCreator.ObjectPool.Get();
            var tr = newPoolItem.transform;
            tr.position = position;
            tr.rotation = Quaternion.identity;

            if (newPoolItem is DamageReceiverPoolItem item)
            {
                item.Init(poolItem =>
                {
                    UFOKilledEvent();

                    if (_objectPoolCreator.CanRelease(poolItem))
                        _objectPoolCreator.ObjectPool.Release(poolItem);
                });
            }
        }
    }
}
