using Windows;
using UnityEngine;
using Interfaces;

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
    }

    private void StartGame()
    {
        _gameInstaller.InstallBindings();
    }

    private void Replay()
    {
        _gameInstaller.ClearBindings();
        
        StartGame();
    }

    private void OnPlayerDead()
    {
        var setUp = new GameOverWindowSetup
        {
            Score = 0,
            WindowSystem = _windowSystem
        };
        _windowSystem.Open<GameOverWindow, GameOverWindowSetup>(setUp);
    }
}
