using Configs;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    [SerializeField] private GameSettingsData _gameSettingsData;
    [SerializeField] private AsteroidSpawner _asteroidObjectSpawner;
    [SerializeField] private ObjectSpawner _smallAsteroidSpawner;
    
    private void OnEnable()
    {
        _asteroidObjectSpawner.PlayerHitAsteroidEvent += OnPlayerHitAsteroidEvent;
    }

    private void OnDisable()
    {
        _asteroidObjectSpawner.PlayerHitAsteroidEvent -= OnPlayerHitAsteroidEvent;
    }

    private void OnPlayerHitAsteroidEvent(Vector3 position)
    {
        for (var i = 0; i < _gameSettingsData.AsteroidOnBulletHitCount; i++)
        {
            var smallAsteroid = _smallAsteroidSpawner.ObjectPool.Get();
            var tr = smallAsteroid.transform;
            tr.rotation = Quaternion.LookRotation(position);
            tr.position = position;
        }
    }
}
