using Configs;
using JetBrains.Annotations;
using UI;
using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    [SerializeField] private GameUpdater _gameUpdater;
    [SerializeField] private PlayerVehicle _playerVehicle;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameSettingsData _gameSettingsData;
    [SerializeField] private ObjectSpawner _bulletSpawner;
    [SerializeField] private ObjectSpawner _asteroidSpawner;
    [SerializeField] private ObjectSpawner _smallAsteroidSpawner;
    [SerializeField] private GameWindow _gameWindow;
    
    [CanBeNull]
    private ServiceLocator _serviceLocator;
    [CanBeNull]
    private InputSystem _inputSystem;
    [CanBeNull]
    private GameObserver _gameObserver;

    public void InstallBindings(GameController gameController)
    {
        _serviceLocator = new ServiceLocator();
        _inputSystem = new InputSystem();
        _gameObserver = new GameObserver();
        
        _bulletSpawner.Init();
        _asteroidSpawner.Init();
        _smallAsteroidSpawner.Init();

        _serviceLocator.AddService(_inputSystem);
        _serviceLocator.AddService(_bulletSpawner);
        _serviceLocator.AddService(_gameSettingsData);
        _serviceLocator.AddService(_gameObserver);
        _serviceLocator.AddService(_smallAsteroidSpawner);
        _serviceLocator.AddService(_playerVehicle);
        _serviceLocator.AddService(_gameUpdater);

        var tr = _playerVehicle.transform;

        var bounds = new Bounds(_mainCamera.transform.position,
            new Vector3(_mainCamera.orthographicSize * _mainCamera.aspect * 2f, _mainCamera.orthographicSize * 2f, 0f));

        var playerAttack = new PlayerAttack(_serviceLocator, bounds, _playerVehicle.FirePositions, _playerVehicle.LaserPosition, tr, _playerVehicle.AttackMaskValue);
        var playerMovement = new PlayerMovement(_mainCamera, tr, _serviceLocator);
        var playerLineRenderer = new LineRendererActivator(_serviceLocator, playerAttack, _playerVehicle.LineRenderer);
        var asteroidSpawner = new AsteroidSpawnerController(_serviceLocator, _asteroidSpawner, _smallAsteroidSpawner, bounds);

        _serviceLocator.AddService(playerMovement);

        _gameUpdater.AddListener(playerMovement);
        _gameUpdater.AddListener(playerLineRenderer);
        _gameUpdater.AddListener(asteroidSpawner);
        
        _gameWindow.Init(_serviceLocator, playerMovement, playerAttack);
        
        _gameObserver.AddListener(playerAttack);
        _gameObserver.AddListener(playerMovement);
        _gameObserver.AddListener(playerLineRenderer);
        _gameObserver.AddListener(_gameWindow);
    }

    public void ClearBindings()
    {
        if (_gameObserver == null || _serviceLocator == null)
            return;
        
        _gameObserver.RemoveAll();
        _serviceLocator.RemoveAll();
    }
}
