using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : IObservable, ILaserAttackable
{
    public event Action<Vector3, Vector3> LaserFireEvent = delegate { };
    
    private readonly List<Transform> _firePositions = new();
    
    private readonly InputSystem _inputSystem;
    private readonly ObjectSpawner _bulletSpawner;
    private readonly Transform _playerTransform;
    private readonly Transform _laserPosition;
    private readonly int _layerMask;

    public PlayerAttack(ServiceLocator serviceLocator, IEnumerable<Transform> firePositions, Transform laserPosition, Transform transform, int attackMask)
    {
        _inputSystem = serviceLocator.GetService<InputSystem>();
        _bulletSpawner = serviceLocator.GetService<BulletSpawner>();
        _firePositions.Clear();
        _firePositions.AddRange(firePositions);
        _playerTransform = transform;
        _laserPosition = laserPosition;
        _layerMask = attackMask;
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

        LaserFireEvent(laserPos, laserPos + _playerTransform.up * 10);
    }
}
