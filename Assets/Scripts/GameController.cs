using Windows;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameInstaller _gameInstaller;
    [SerializeField] private Player _player;
    [SerializeField] private WindowSystem _windowSystem;

    private void OnEnable()
    {
        _player.DeadEvent += OnPlayerDead;
        _windowSystem.CloseWindowEvent += OnCloseWindow;
    }

    private void OnDisable()
    {
        _player.DeadEvent -= OnPlayerDead;
        _windowSystem.CloseWindowEvent -= OnCloseWindow;
    }

    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _gameInstaller.InstallBindings();
        
        if (_gameInstaller.ServiceLocator == null)
            return;

        _gameInstaller.ServiceLocator.GetService<GameListener>().StartGame();

        var windowSetup = new GameWindowSetup
        {
            ServiceLocator = _gameInstaller.ServiceLocator
        };
        _windowSystem.Open<GameWindow, GameWindowSetup>(windowSetup);
    }

    private void OnCloseWindow(ASimpleWindow windowType)
    {
        if (windowType is not GameOverWindow)
            return;
        
        _gameInstaller.ClearBindings();
        
        _windowSystem.Close<GameWindow>();
        
        StartGame();
    }

    private void OnPlayerDead()
    {
        if (_gameInstaller.ServiceLocator == null)
            return;
        
        var pointsController = _gameInstaller.ServiceLocator.GetService<PointsController>();
        _gameInstaller.ServiceLocator.GetService<GameListener>().FinishGame();

        var windowSetup = new GameOverWindowSetup
        {
            Score = pointsController.CurrentCoins,
            WindowSystem = _windowSystem
        };
        _windowSystem.Open<GameOverWindow, GameOverWindowSetup>(windowSetup);
    }
}
