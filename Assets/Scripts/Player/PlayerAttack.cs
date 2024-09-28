using System;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Pools;
using InputSystem = Game.InputSystem;

namespace Player
{
    public class PlayerAttack : IDisposable
    {
        private readonly InputSystem _inputSystem;
        private readonly Transform _playerTransform;
        private readonly AttackerObjectPoolFactory _bulletPoolFactory;

        private readonly List<Transform> _firePositions = new();

        public PlayerAttack(ServiceLocator serviceLocator, AttackerObjectPoolFactory bulletPoolFactory, IEnumerable<Transform> firePositions, Transform transform)
        {
            _inputSystem = serviceLocator.GetService<InputSystem>();
            _bulletPoolFactory = bulletPoolFactory;

            _playerTransform = transform;

            _firePositions.Clear();
            _firePositions.AddRange(firePositions);

            _inputSystem.Player.BulletFire.performed += OnBulletAttack;
        }

        public void Dispose()
        {
            _inputSystem.Player.BulletFire.performed -= OnBulletAttack;
        }

        private void OnBulletAttack(InputAction.CallbackContext value)
        {
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
