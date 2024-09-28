using System;
using Configs;
using Game;
using Pools;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Spawners
{
    public class SmallAsteroidSpawner : IDisposable
    {
        public event Action SmallAsteroidKilledEvent = delegate { };

        private readonly DamageReceiverFactory _objectFactory;
        private readonly AsteroidSpawner _asteroidSpawner;

        private readonly Vector2 _angleRange;
        private readonly int _extraAsteroidCounts;

        public SmallAsteroidSpawner(ServiceLocator serviceLocator, DamageReceiverFactory objectFactory)
        {
            _objectFactory = objectFactory;

            _asteroidSpawner = serviceLocator.GetService<AsteroidSpawner>();
            var data = serviceLocator.GetService<GameSettingsData>();

            _angleRange = data.SmallAsteroidAttackAngleRange;
            _extraAsteroidCounts = data.AsteroidOnBulletHitCount;
            
            _asteroidSpawner.AsteroidKilledEvent += SpawnAsteroid;
        }

        public void Dispose()
        {
            _asteroidSpawner.AsteroidKilledEvent -= SpawnAsteroid;
        }

        private void SpawnAsteroid(Vector3 position, Quaternion rotation)
        {
            for (var i = 0; i < _extraAsteroidCounts; i++)
            {
                var newRotation = Quaternion.Euler(new Vector3(0f, 0f, rotation.eulerAngles.z * Random.Range(_angleRange.x, _angleRange.y)));

                var newAsteroid = _objectFactory.ObjectPool.Get();
                var tr = newAsteroid.transform;
                tr.position = position;
                tr.rotation = newRotation;
                
                newAsteroid.Init(poolItem =>
                {
                    SmallAsteroidKilledEvent();

                    if (ObjectPoolFactory.CanRelease(poolItem))
                        _objectFactory.ObjectPool.Release(poolItem);
                });
            }
        }
    }
}
