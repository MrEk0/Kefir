using Windows;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameInstaller _gameInstaller;
    [SerializeField] private PlayerVehicle _playerVehicle;
    [SerializeField] private WindowSystem _windowSystem;
    
    private void OnEnable()
    {
        _playerVehicle.DeadEvent += OnPlayerDead;
    }

    private void OnDisable()
    {
        _playerVehicle.DeadEvent -= OnPlayerDead;
    }

    private void Start()
    {
        StartGame();

        if (_gameInstaller.ServiceLocator == null)
            return;

        var windowSetup = new GameWindowSetup { ServiceLocator = _gameInstaller.ServiceLocator };
        _windowSystem.Open<GameWindow, GameWindowSetup>(windowSetup);
    }

    private void StartGame()
    {
        _gameInstaller.InstallBindings();
    }

    private void Replay()
    {
        _gameInstaller.ClearBindings();
        
        _windowSystem.Close<GameWindow>();
        
        StartGame();
    }

    private void OnPlayerDead()
    {
        var windowSetup = new GameOverWindowSetup
        {
            Score = 0,
            WindowSystem = _windowSystem
        };
        _windowSystem.Open<GameOverWindow, GameOverWindowSetup>(windowSetup);
    }
}
