using System;
using Configs;
using Game;
using Interfaces;
using JetBrains.Annotations;
using Player;
using Pools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawners
{
    public class AsteroidSpawner : IGameUpdatable, IServisable
    {
        public event Action<Vector3, Quaternion> AsteroidKilledEvent = delegate { };

        [CanBeNull] private readonly AttackerObjectPoolCreator _asteroidPoolCreator;
        [CanBeNull] private readonly PlayerVehicle _player;

        private readonly float _pause;
        private readonly Vector2 _angleRange;

        private float _timer;
        private Bounds _bounds;

        public AsteroidSpawner(ServiceLocator serviceLocator, AttackerObjectPoolCreator asteroidPoolCreator,
            Bounds bounds)
        {
            _asteroidPoolCreator = asteroidPoolCreator;
            _bounds = bounds;

            _player = serviceLocator.GetService<PlayerVehicle>();
            var data = serviceLocator.GetService<GameSettingsData>();

            _pause = data.AsteroidSpawnPause;
            _angleRange = data.AsteroidAttackAngleRange;
        }

        public void OnUpdate(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer < _pause)
                return;

            SpawnNewAsteroid();

            _timer = 0f;
        }

        private void SpawnNewAsteroid()
        {
            if (_player == null || _asteroidPoolCreator == null)
                return;

            var position = new Vector3(Random.Range(_bounds.min.x, _bounds.max.x), _bounds.max.y, 0f);

            var direction = (_player.transform.position - position).normalized;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            var rotation = Quaternion.Euler(new Vector3(0f, 0f, angle * Random.Range(_angleRange.x, _angleRange.y)));

            var newAsteroid = _asteroidPoolCreator.ObjectPool.Get();
            var tr = newAsteroid.transform;
            tr.position = position;
            tr.rotation = rotation;

            if (newAsteroid is DamageReceiverPoolItem item)
            {
                item.Init(poolItem =>
                {
                    var poolItemTransform = poolItem.transform;

                    AsteroidKilledEvent(poolItemTransform.position, poolItemTransform.rotation);

                    if (_asteroidPoolCreator.CanRelease(poolItem))
                        _asteroidPoolCreator.ObjectPool.Release(poolItem);
                });
            }
        }
    }
}
