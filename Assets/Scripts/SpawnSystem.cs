using System;
using System.Collections.Generic;
using Configs;
using Interfaces;
using UnityEngine;

public class SpawnSystem : IObservable, IServisable
{
    public event Action<int> ObjectKilledEvent = delegate { };
    
    private AttackerObjectPoolCreator _bulletPoolCreator;
    private AsteroidSpawner _asteroidSpawner;
    private SmallAsteroidSpawner _smallAsteroidSpawner;
    private UFOSpawner _ufoSpawner;
    private ServiceLocator _serviceLocator;
    
    public SpawnSystem(ServiceLocator serviceLocator, AttackerObjectPoolCreator asteroidPoolCreator,
        AttackerObjectPoolCreator smallAsteroidPoolCreator, AttackerObjectPoolCreator bulletPoolCreator,
        TargetFollowerObjectPoolCreator ufoPoolCreator, Bounds screenBounds)
    {
        _bulletPoolCreator = bulletPoolCreator;
        _serviceLocator = serviceLocator;
        
        var activeObjectBounds = new Bounds(screenBounds.center, screenBounds.size * 1.5f);//todo change
        
        _bulletPoolCreator.Init(activeObjectBounds, serviceLocator);
        asteroidPoolCreator.Init(activeObjectBounds, serviceLocator);
        smallAsteroidPoolCreator.Init(activeObjectBounds, serviceLocator);
        ufoPoolCreator.Init(activeObjectBounds, serviceLocator.GetService<PlayerVehicle>().transform, serviceLocator);
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
        _ufoSpawner.UFOKilledEvent += OnUFOKilled;
    }

    public void Unsubscribe()
    {
        _asteroidSpawner.AsteroidKilledEvent -= OnAsteroidKilled;
        _smallAsteroidSpawner.SmallAsteroidKilledEvent -= OnSmallAsteroidKilled;
        _ufoSpawner.UFOKilledEvent -= OnUFOKilled;
    }

    private void OnAsteroidKilled(Vector3 position, Quaternion rotation)
    {
        ObjectKilledEvent(_serviceLocator.GetService<GameSettingsData>().CoinsForAsteroid);
    }

    private void OnSmallAsteroidKilled()
    {
        ObjectKilledEvent(_serviceLocator.GetService<GameSettingsData>().CoinsForSmallAsteroid);
    }

    private void OnUFOKilled()
    {
        ObjectKilledEvent(_serviceLocator.GetService<GameSettingsData>().CoinsForUFO);
    }
}
