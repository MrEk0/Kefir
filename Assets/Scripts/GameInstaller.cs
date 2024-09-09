using Configs;
using JetBrains.Annotations;
using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    [SerializeField] private GameUpdater _gameUpdater;
    [SerializeField] private PlayerVehicle _playerVehicle;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameSettingsData _gameSettingsData;
    [SerializeField] private AttackerObjectPoolCreator _bulletPoolCreator;
    [SerializeField] private AttackerObjectPoolCreator _asteroidPoolCreator;
    [SerializeField] private AttackerObjectPoolCreator _smallAsteroidPoolCreator;
    [SerializeField] private TargetFollowerObjectPoolCreator _ufoPoolCreator;

    [CanBeNull] private InputSystem _inputSystem;
    [CanBeNull] private GameObserver _gameObserver;

    [CanBeNull]
    public ServiceLocator ServiceLocator { get; private set; }

    public void InstallBindings()
    {
        var tr = _playerVehicle.transform;
        var bounds = new Bounds(_mainCamera.transform.position,
            new Vector3(_mainCamera.orthographicSize * _mainCamera.aspect * 2f, _mainCamera.orthographicSize * 2f, 0f));
        
        ServiceLocator = new ServiceLocator();
        _inputSystem = new InputSystem();
        _gameObserver = new GameObserver();

        ServiceLocator.AddService(_inputSystem);
        ServiceLocator.AddService(_gameSettingsData);
        ServiceLocator.AddService(_gameObserver);
        ServiceLocator.AddService(_playerVehicle);
        ServiceLocator.AddService(_gameUpdater);

        var spawnSystem = new SpawnSystem(ServiceLocator, _asteroidPoolCreator, _smallAsteroidPoolCreator, _bulletPoolCreator, _ufoPoolCreator, bounds);
        ServiceLocator.AddService(spawnSystem);
        _gameObserver.AddListener(spawnSystem);
        
        var asteroidSpawner = new AsteroidSpawner(ServiceLocator, _asteroidPoolCreator, bounds);
        ServiceLocator.AddService(asteroidSpawner);
        _gameUpdater.AddListener(asteroidSpawner);
        
        var smallAsteroidSpawner = new SmallAsteroidSpawner(ServiceLocator, _smallAsteroidPoolCreator);
        _gameObserver.AddListener(smallAsteroidSpawner);

        var ufoSpawner = new UFOSpawner(ServiceLocator, _ufoPoolCreator, bounds);
        ServiceLocator.AddService(ufoSpawner);
        _gameUpdater.AddListener(ufoSpawner);
        
        var playerAttack = new PlayerAttack(ServiceLocator, bounds, _playerVehicle.FirePositions, tr);
        _gameObserver.AddListener(playerAttack);
        
        var playerLaserAttack = new PlayerLaserAttack(ServiceLocator, bounds, _playerVehicle.LaserPosition, tr);
        ServiceLocator.AddService(playerLaserAttack);
        _gameUpdater.AddListener(playerLaserAttack);
        _gameObserver.AddListener(playerLaserAttack);
        
        var playerMovement = new PlayerMovement(_mainCamera, tr, ServiceLocator);
        ServiceLocator.AddService(playerMovement);
        _gameUpdater.AddListener(playerMovement);
        _gameObserver.AddListener(playerMovement);
        
        var playerLineRenderer = new LineRendererActivator(ServiceLocator, playerLaserAttack, _playerVehicle.LineRenderer);
        _gameUpdater.AddListener(playerLineRenderer);
        _gameObserver.AddListener(playerLineRenderer);
    }

    public void ClearBindings()
    {
        if (_gameObserver == null || ServiceLocator == null)
            return;

        _gameObserver.RemoveAll();
        ServiceLocator.RemoveAll();
        _gameUpdater.RemoveAll();
    }
}
