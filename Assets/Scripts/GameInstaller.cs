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
    [SerializeField] private AttackerObjectPoolCreator bulletPoolCreator;
    [SerializeField] private AttackerObjectPoolCreator asteroidPoolCreator;
    [SerializeField] private AttackerObjectPoolCreator smallAsteroidPoolCreator;
    [SerializeField] private TargetFollowerObjectPoolCreator ufoPoolCreator;
    [SerializeField] private GameWindow _gameWindow;
    
    [CanBeNull]
    private ServiceLocator _serviceLocator;
    [CanBeNull]
    private InputSystem _inputSystem;
    [CanBeNull]
    private GameObserver _gameObserver;

    public void InstallBindings()
    {
        var tr = _playerVehicle.transform;
        var bounds = new Bounds(_mainCamera.transform.position,
            new Vector3(_mainCamera.orthographicSize * _mainCamera.aspect * 2f, _mainCamera.orthographicSize * 2f, 0f));
        
        _serviceLocator = new ServiceLocator();
        _inputSystem = new InputSystem();
        _gameObserver = new GameObserver();

        _serviceLocator.AddService(_inputSystem);
        _serviceLocator.AddService(_gameSettingsData);
        _serviceLocator.AddService(_gameObserver);
        _serviceLocator.AddService(_playerVehicle);
        _serviceLocator.AddService(_gameUpdater);

        var spawnSystem = new SpawnSystem(_serviceLocator, asteroidPoolCreator, smallAsteroidPoolCreator, bulletPoolCreator, ufoPoolCreator, bounds);
        _serviceLocator.AddService(spawnSystem);
        _gameObserver.AddListener(spawnSystem);
        
        var playerAttack = new PlayerAttack(_serviceLocator, bounds, _playerVehicle.FirePositions, _playerVehicle.LaserPosition, tr);
        _gameObserver.AddListener(playerAttack);
        
        var playerMovement = new PlayerMovement(_mainCamera, tr, _serviceLocator);
        _serviceLocator.AddService(playerMovement);
        _gameUpdater.AddListener(playerMovement);
        _gameObserver.AddListener(playerMovement);
        
        var playerLineRenderer = new LineRendererActivator(_serviceLocator, playerAttack, _playerVehicle.LineRenderer);
        _gameUpdater.AddListener(playerLineRenderer);
        _gameObserver.AddListener(playerLineRenderer);

        _gameWindow.Init(_serviceLocator, playerMovement, playerAttack);
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
