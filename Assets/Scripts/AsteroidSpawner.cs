using System;
using UnityEngine;

public class AsteroidSpawner : ObjectSpawner
{
    public event Action<Vector3> PlayerHitAsteroidEvent = delegate { };

    protected override void MakeCreate(ObjectSpawnerItem item)
    {
        if (item is AsteroidSpawnerItem asteroidSpawnerItem)
        {
            asteroidSpawnerItem.PlayerHitEvent += OnPlayerHitEvent;
        }
    }

    protected override void MakeDestroy(ObjectSpawnerItem item)
    {
        if (item is AsteroidSpawnerItem asteroidSpawnerItem)
        {
            asteroidSpawnerItem.PlayerHitEvent -= OnPlayerHitEvent;
        }
    }

    private void OnPlayerHitEvent(Vector3 position)
    {
        PlayerHitAsteroidEvent(position);
    }
}
