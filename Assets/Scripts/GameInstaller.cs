using Configs;
using JetBrains.Annotations;
using UI;
using UnityEngine;

public class GameInstaller : MonoBehaviour
{
    [SerializeField] private PlayerVehicle _playerVehicle;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private GameSettingsData _gameSettingsData;
    [SerializeField] private BulletSpawner _bulletSpawner;
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
        
        _serviceLocator.AddService(_inputSystem);
        _serviceLocator.AddService(_bulletSpawner);
        _serviceLocator.AddService(_gameSettingsData);
        _serviceLocator.AddService(_gameObserver);

        var tr = _playerVehicle.transform;

        var playerAttack = new PlayerAttack(_serviceLocator, _playerVehicle.FirePositions, _playerVehicle.LaserPosition, tr, _playerVehicle.AttackMaskValue);
        var playerMovement = new PlayerMovement(_mainCamera, tr, _serviceLocator);
        var playerLineRenderer = new LineRendererActivator(_serviceLocator, playerAttack, _playerVehicle.LineRenderer);

        _serviceLocator.AddService(playerMovement);

        gameController.AddListener(playerMovement);
        
        _gameObserver.AddListener(playerAttack);
        _gameObserver.AddListener(playerMovement);
        _gameObserver.AddListener(playerLineRenderer);

        _gameWindow.Init(playerMovement, playerAttack);
    }

    public void ClearBindings()
    {
        if (_gameObserver == null || _serviceLocator == null)
            return;
        
        _gameObserver.RemoveAll();
        _serviceLocator.RemoveAll();
    }
}
