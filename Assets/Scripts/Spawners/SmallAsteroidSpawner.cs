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
    public class SmallAsteroidSpawner : ISubscribable, IServisable
    {
        public event Action SmallAsteroidKilledEvent = delegate { };

        [CanBeNull] private readonly AttackerObjectPoolCreator _objectPoolCreator;
        [CanBeNull] private readonly AsteroidSpawner _asteroidSpawner;

        private readonly Vector2 _angleRange;
        private readonly int _extraAsteroidCounts;

        public SmallAsteroidSpawner(ServiceLocator serviceLocator, AttackerObjectPoolCreator objectPoolCreator)
        {
            _objectPoolCreator = objectPoolCreator;

            _asteroidSpawner = serviceLocator.GetService<AsteroidSpawner>();
            var data = serviceLocator.GetService<GameSettingsData>();

            _angleRange = data.SmallAsteroidAttackAngleRange;
            _extraAsteroidCounts = data.AsteroidOnBulletHitCount;
        }

        public void Subscribe()
        {
            if (_asteroidSpawner == null)
                return;

            _asteroidSpawner.AsteroidKilledEvent += SpawnAsteroid;
        }

        public void Unsubscribe()
        {
            if (_asteroidSpawner == null)
                return;

            _asteroidSpawner.AsteroidKilledEvent -= SpawnAsteroid;
        }

        private void SpawnAsteroid(Vector3 position, Quaternion rotation)
        {
            if (_objectPoolCreator == null)
                return;

            for (var i = 0; i < _extraAsteroidCounts; i++)
            {
                var newRotation = Quaternion.Euler(new Vector3(0f, 0f,
                    rotation.eulerAngles.z * Random.Range(_angleRange.x, _angleRange.y)));

                var newAsteroid = _objectPoolCreator.ObjectPool.Get();
                var tr = newAsteroid.transform;
                tr.position = position;
                tr.rotation = newRotation;

                if (newAsteroid is DamageReceiverPoolItem item)
                {
                    item.Init(poolItem =>
                    {
                        SmallAsteroidKilledEvent();

                        if (_objectPoolCreator.CanRelease(poolItem))
                            _objectPoolCreator.ObjectPool.Release(poolItem);
                    });
                }
            }
        }
    }
}
