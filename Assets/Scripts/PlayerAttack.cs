using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;

public class PlayerAttack : IObservable, ILaserAttackable
{
    public event Action<Vector3, Vector3> LaserFireEvent = delegate { };
    public event Action EnemyKillEvent = delegate { };
    
    private readonly List<Transform> _firePositions = new();
    
    private readonly InputSystem _inputSystem;
    private readonly ObjectSpawner _bulletSpawner;
    private readonly Transform _playerTransform;
    private readonly Transform _laserPosition;
    private readonly int _layerMask;
    private readonly Bounds _screenBounds;

    public PlayerAttack(ServiceLocator serviceLocator, Bounds bounds, IEnumerable<Transform> firePositions, Transform laserPosition, Transform transform, int attackMask)
    {
        _inputSystem = serviceLocator.GetService<InputSystem>();
        _bulletSpawner = serviceLocator.GetService<ObjectSpawner>();
        
        _playerTransform = transform;
        _laserPosition = laserPosition;
        _layerMask = attackMask;
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
        foreach (var tr in _firePositions)
        {
            var bullet = _bulletSpawner.ObjectPool.Get();
            
            var bulletTr = bullet.transform;
            bulletTr.position = tr.position;
            bulletTr.rotation = _playerTransform.rotation;
            
            bullet.Init(new Bounds(_screenBounds.center, _screenBounds.size * 1.5f), other =>
            {
                EnemyKillEvent();
                
                var target = other.transform.GetComponent<IDamagable>();
                target?.TakeDamage();
            });
        }
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
        EnemyKillEvent();
    }
}
