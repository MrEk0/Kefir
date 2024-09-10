using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;

public class PlayerAttack : ISubscribable
{
    private readonly InputSystem _inputSystem;
    private readonly Transform _playerTransform;
    private readonly AttackerObjectPoolCreator _bulletPoolCreator;

    private readonly List<Transform> _firePositions = new();

    public PlayerAttack(ServiceLocator serviceLocator, AttackerObjectPoolCreator bulletPoolCreator, IEnumerable<Transform> firePositions, Transform transform)
    {
        _inputSystem = serviceLocator.GetService<InputSystem>();
        _bulletPoolCreator = bulletPoolCreator;

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
        foreach (var tr in _firePositions)
        {
            var bullet = _bulletPoolCreator.ObjectPool.Get();
            
            var bulletTr = bullet.transform;
            bulletTr.position = tr.position;
            bulletTr.rotation = _playerTransform.rotation;
        }
    }
}
