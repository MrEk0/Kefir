using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;

public class PlayerAttack : IObservable
{
    private readonly InputSystem _inputSystem;
    private readonly SpawnSystem _spawnSystem;
    private readonly Transform _playerTransform;

    private readonly List<Transform> _firePositions = new();

    public PlayerAttack(ServiceLocator serviceLocator, Bounds bounds, IEnumerable<Transform> firePositions, Transform transform)
    {
        _inputSystem = serviceLocator.GetService<InputSystem>(); 
        _spawnSystem = serviceLocator.GetService<SpawnSystem>();
        
        _playerTransform = transform;

        _firePositions.Clear();
        _firePositions.AddRange(firePositions);
    }

    public void Subscribe()
    {
        _inputSystem.Player.BulletFire.performed += OnBulletAttack;
    }

    public void Unsubscribe()
    {
        _inputSystem.Player.BulletFire.performed -= OnBulletAttack;
    }

    private void OnBulletAttack(InputAction.CallbackContext value)
    {
        _spawnSystem.SpawnPlayerProjectiles(_firePositions, _playerTransform);
    }
}
