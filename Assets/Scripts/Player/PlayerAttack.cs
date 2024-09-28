using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using JetBrains.Annotations;
using Pools;
using InputSystem = Game.InputSystem;

namespace Player
{
    public class PlayerAttack : IDisposable
    {
        [CanBeNull] private readonly InputSystem _inputSystem;
        [CanBeNull] private readonly Transform _playerTransform;
        [CanBeNull] private readonly AttackerObjectPoolFactory _bulletPoolFactory;

        private readonly List<Transform> _firePositions = new();

        public PlayerAttack(ServiceLocator serviceLocator, AttackerObjectPoolFactory bulletPoolFactory, IEnumerable<Transform> firePositions, Transform transform)
        {
            _inputSystem = serviceLocator.GetService<InputSystem>();
            _bulletPoolFactory = bulletPoolFactory;

            _playerTransform = transform;

            _firePositions.Clear();
            _firePositions.AddRange(firePositions);

            if (_inputSystem == null)
                return;

            _inputSystem.Player.BulletFire.performed += OnBulletAttack;
        }

        public void Dispose()
        {
            if (_inputSystem == null)
                return;

            _inputSystem.Player.BulletFire.performed -= OnBulletAttack;
        }

        private void OnBulletAttack(InputAction.CallbackContext value)
        {
            if (_bulletPoolFactory == null || _playerTransform == null)
                return;

            foreach (var tr in _firePositions)
            {
                var bullet = _bulletPoolFactory.ObjectPool.Get();

                var bulletTr = bullet.transform;
                bulletTr.position = tr.position;
                bulletTr.rotation = _playerTransform.rotation;
            }
        }
    }
}
