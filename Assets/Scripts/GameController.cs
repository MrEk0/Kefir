using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameInstaller _gameInstaller;
    [SerializeField] private BulletSpawner _bulletSpawner;

    private readonly List<IGameUpdatable> _updateListeners = new();
    
    private void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        _gameInstaller.InstallBindings(this);

        _bulletSpawner.Init();
    }

    private void Replay()
    {
        _gameInstaller.ClearBindings();
        
        StartGame();
    }

    public void AddListener(IGameUpdatable listener)
    {
        _updateListeners.Add(listener);
    }
    
    public void RemoveListener(IGameUpdatable listener)
    {
        _updateListeners.Remove(listener);
    }
    
    private void Update()
    {
        var deltaTime = Time.deltaTime;
        for (var i = 0; i < _updateListeners.Count; i++)
        {
            _updateListeners[i].OnUpdate(deltaTime);
        }
    }
}
