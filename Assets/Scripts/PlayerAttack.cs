using System;
using System.Collections.Generic;
using Configs;
using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;

public class PlayerAttack : IObservable, ILaserAttackable
{
    public event Action<Vector3, Vector3> LaserFireEvent = delegate { };
    
    private readonly InputSystem _inputSystem;
    private readonly SpawnSystem _spawnSystem;
    private readonly Transform _playerTransform;
    private readonly Transform _laserPosition;
    private readonly Bounds _screenBounds;
    private readonly int _layerMask;
    
    private readonly List<Transform> _firePositions = new();

    public PlayerAttack(ServiceLocator serviceLocator, Bounds bounds, IEnumerable<Transform> firePositions, Transform laserPosition, Transform transform)
    {
        _inputSystem = serviceLocator.GetService<InputSystem>();
        _spawnSystem = serviceLocator.GetService<SpawnSystem>();
        
        _playerTransform = transform;
        _laserPosition = laserPosition;
        _layerMask = serviceLocator.GetService<GameSettingsData>().PlayerAttackMask.value;
        _screenBounds = bounds;
        
        _firePositions.Clear();
        _firePositions.AddRange(firePositions);
    }

    public void Subscribe()
    {
        _inputSystem.Player.BulletFire.performed += OnBulletAttack;
        _inputSystem.Player.LaserFire.performed += OnLaserAttack;
    }

    public void Unsubscribe()
    {
        _inputSystem.Player.BulletFire.performed -= OnBulletAttack;
        _inputSystem.Player.LaserFire.performed -= OnLaserAttack;
    }

    private void OnBulletAttack(InputAction.CallbackContext value)
    {
        _spawnSystem.SpawnPlayerProjectiles(_firePositions, _playerTransform);
    }

    private void OnLaserAttack(InputAction.CallbackContext value)
    {
        var laserPos = _laserPosition.position;
        var results = Physics2D.RaycastAll(laserPos, _playerTransform.up, float.PositiveInfinity, _layerMask);

        foreach (var result in results)
        {
            var target = result.transform.GetComponent<IDamagable>();
            target?.TakeDamage();
        }
        
        LaserFireEvent(laserPos, laserPos + _playerTransform.up * Vector2.Distance(_screenBounds.max, _screenBounds.center));
    }
}
