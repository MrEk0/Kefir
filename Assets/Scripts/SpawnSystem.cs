using System;
using System.Collections.Generic;
using Configs;
using Interfaces;
using UnityEngine;

public class SpawnSystem : IObservable, IServisable
{
    public event Action<int> ObjectKilledEvent = delegate { };
    
    private AttackableObjectPoolCreator _bulletPoolCreator;
    private AsteroidSpawner _asteroidSpawner;
    private SmallAsteroidSpawner _smallAsteroidSpawner;
    private ServiceLocator _serviceLocator;
    
    public SpawnSystem(ServiceLocator serviceLocator, AttackableObjectPoolCreator asteroidPoolCreator,
        AttackableObjectPoolCreator smallAsteroidPoolCreator, AttackableObjectPoolCreator bulletPoolCreator,
        Bounds screenBounds)
    {
        _bulletPoolCreator = bulletPoolCreator;
        _serviceLocator = serviceLocator;
        
        var gameUpdater = serviceLocator.GetService<GameUpdater>();
        var gameObserver = serviceLocator.GetService<GameObserver>();
        var activeObjectBounds = new Bounds(screenBounds.center, screenBounds.size * 1.5f);
        
        _bulletPoolCreator.Init(activeObjectBounds, serviceLocator);
        asteroidPoolCreator.Init(activeObjectBounds, serviceLocator);
        smallAsteroidPoolCreator.Init(activeObjectBounds, serviceLocator);

        _asteroidSpawner = new AsteroidSpawner(serviceLocator, asteroidPoolCreator, screenBounds);
        serviceLocator.AddService(_asteroidSpawner);
        
        _smallAsteroidSpawner = new SmallAsteroidSpawner(serviceLocator, smallAsteroidPoolCreator);
        serviceLocator.AddService(_smallAsteroidSpawner);
        
        gameUpdater.AddListener(_asteroidSpawner);
        gameObserver.AddListener(_smallAsteroidSpawner);
    }

    public void SpawnPlayerProjectiles(IEnumerable<Transform> firePositions, Transform player)
    {
        foreach (var tr in firePositions)
        {
            var bullet = _bulletPoolCreator.ObjectPool.Get();
            
            var bulletTr = bullet.transform;
            bulletTr.position = tr.position;
            bulletTr.rotation = player.rotation;
        }
    }

    public void Subscribe()
    {
        _asteroidSpawner.AsteroidKilledEvent += OnAsteroidKilled;
        _smallAsteroidSpawner.SmallAsteroidKilledEvent += OnSmallAsteroidKilled;
    }

    public void Unsubscribe()
    {
        _asteroidSpawner.AsteroidKilledEvent -= OnAsteroidKilled;
        _smallAsteroidSpawner.SmallAsteroidKilledEvent -= OnSmallAsteroidKilled;
    }

    private void OnAsteroidKilled(Vector3 position, Quaternion rotation)
    {
        ObjectKilledEvent(_serviceLocator.GetService<GameSettingsData>().CoinsForAsteroid);
    }

    private void OnSmallAsteroidKilled()
    {
        ObjectKilledEvent(_serviceLocator.GetService<GameSettingsData>().CoinsForSmallAsteroid);
    }
}
