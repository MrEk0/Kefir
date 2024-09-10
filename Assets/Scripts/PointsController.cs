using System;
using Configs;
using Interfaces;
using UnityEngine;

public class PointsController : IServisable, ISubscribable
{
    public event Action<int> PointsUpdateEvent = delegate { };
    
    private readonly AsteroidSpawner _asteroidSpawner;
    private readonly SmallAsteroidSpawner _smallAsteroidSpawner;
    private readonly UFOSpawner _ufoSpawner;

    private readonly int _asteroidCoins;
    private readonly int _smallAsteroidCoins;
    private readonly int _ufoCoins;

    public int CurrentCoins { get; private set; }

    public PointsController(ServiceLocator serviceLocator)
    {
        var data = serviceLocator.GetService<GameSettingsData>();
        
        _asteroidSpawner = serviceLocator.GetService<AsteroidSpawner>();
        _smallAsteroidSpawner = serviceLocator.GetService<SmallAsteroidSpawner>();
        _ufoSpawner = serviceLocator.GetService<UFOSpawner>();

        _asteroidCoins = data.CoinsForAsteroid;
        _smallAsteroidCoins = data.CoinsForSmallAsteroid;
        _ufoCoins = data.CoinsForUFO;
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
        CurrentCoins += _asteroidCoins;
        
        PointsUpdateEvent(CurrentCoins);
    }

    private void OnSmallAsteroidKilled()
    {
        CurrentCoins += _smallAsteroidCoins;
        
        PointsUpdateEvent(CurrentCoins);
    }

    private void OnUFOKilled()
    {
        CurrentCoins += _ufoCoins;
        
        PointsUpdateEvent(CurrentCoins);
    }
}
