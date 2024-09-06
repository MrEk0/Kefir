using Configs;
using Interfaces;
using UnityEngine;

public class AsteroidSpawnerController : IGameUpdatable
{
    private readonly ObjectSpawner _asteroidSpawner;
    private readonly ObjectSpawner _smallAsteroidSpawner;
    private readonly PlayerVehicle _playerVehicle;
    
    private readonly float _pause;
    private readonly Vector2 _angleRange;
    
    private float _timer;
    private Bounds _bounds;
    private readonly int _asteroidCount;

    public AsteroidSpawnerController(ServiceLocator serviceLocator, ObjectSpawner asteroidSpawner, ObjectSpawner smallAsteroidSpawner, Bounds bounds)
    {
        _asteroidSpawner = asteroidSpawner;
        _smallAsteroidSpawner = smallAsteroidSpawner;
        _bounds = bounds;

        _playerVehicle = serviceLocator.GetService<PlayerVehicle>();
        var data = serviceLocator.GetService<GameSettingsData>();
        
        _pause = data.AsteroidSpawnPause;
        _angleRange = data.AsteroidTargetAngleRange;
        _asteroidCount = data.AsteroidOnBulletHitCount;
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
        var position = new Vector3(Random.Range(_bounds.min.x, _bounds.max.x), _bounds.max.y, 0f);
        
        var direction = (_playerVehicle.transform.position - position).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        var rotation = Quaternion.Euler(new Vector3(0f, 0f, angle * Random.Range(_angleRange.x, _angleRange.y)));
        
        var newAsteroid = _asteroidSpawner.ObjectPool.Get();
        var tr = newAsteroid.transform;
        tr.position = position;
        tr.rotation = rotation;
        
        newAsteroid.Init(new Bounds(_bounds.center, _bounds.size * 1.5f),other =>
        {
            var target = other.transform.GetComponent<IDamagable>();
            target?.TakeDamage();

            OnPlayerHitAsteroidEvent(other.transform.position);
        });
    }
    
    private void OnPlayerHitAsteroidEvent(Vector3 position)
    {
        for (var i = 0; i < _asteroidCount; i++)
        {
            var smallAsteroid = _smallAsteroidSpawner.ObjectPool.Get();
            var tr = smallAsteroid.transform;
            tr.rotation = Quaternion.LookRotation(position);
            tr.position = position;
        }
    }
}
