using System;
using Configs;
using Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

public class SmallAsteroidSpawner : ISubscribable, IServisable
{
    public event Action SmallAsteroidKilledEvent = delegate { };
    
    private readonly AttackerObjectPoolCreator _objectPoolCreator;
    private readonly AsteroidSpawner _asteroidSpawner;

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
        _asteroidSpawner.AsteroidKilledEvent += SpawnAsteroid;
    }

    public void Unsubscribe()
    {
        _asteroidSpawner.AsteroidKilledEvent -= SpawnAsteroid;
    }
    
    private void SpawnAsteroid(Vector3 position, Quaternion rotation)
    {
        for (var i = 0; i < _extraAsteroidCounts; i++)
        {
            var newRotation = Quaternion.Euler(new Vector3(0f, 0f, rotation.eulerAngles.z * Random.Range(_angleRange.x, _angleRange.y)));

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
