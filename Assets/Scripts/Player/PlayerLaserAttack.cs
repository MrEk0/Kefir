using System;
using Configs;
using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;
using JetBrains.Annotations;
using InputSystem = Game.InputSystem;

namespace Player
{
    public class PlayerLaserAttack : ISubscribable, ILaserAttackable, IGameUpdatable, IServisable
    {
        public event Action<Vector3, Vector3> LaserFireEvent = delegate { };
        public event Action<float> LaserTimerEvent = delegate { };
        public event Action<int> LaserShotEvent = delegate { };

        [CanBeNull] private readonly InputSystem _inputSystem;
        [CanBeNull] private readonly Transform _playerTransform;
        [CanBeNull] private readonly Transform _laserPosition;

        private readonly Bounds _screenBounds;
        private readonly int _layerMask;
        private readonly int _maxLaserShotCount;
        private readonly float _laserShotRecoveryTime;

        private int _laserShotsCount;
        private float _timer;

        public PlayerLaserAttack(ServiceLocator serviceLocator, Bounds bounds, Transform laserPosition,
            Transform transform)
        {
            _inputSystem = serviceLocator.GetService<InputSystem>();

            var data = serviceLocator.GetService<GameSettingsData>();

            _playerTransform = transform;
            _laserPosition = laserPosition;
            _layerMask = data.PlayerAttackMask.value;
            _screenBounds = bounds;
            _maxLaserShotCount = data.MaxLaserShotCount;
            _laserShotRecoveryTime = data.LaserRecoveryTime;
            _laserShotsCount = _maxLaserShotCount;
            _timer = _laserShotRecoveryTime;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_laserShotsCount == _maxLaserShotCount)
                return;

            _timer -= deltaTime;
            LaserTimerEvent(Mathf.Max(0f, _timer));

            if (_timer > 0f)
                return;

            _laserShotsCount = Mathf.Min(_maxLaserShotCount, _laserShotsCount + 1);
            _timer = _laserShotRecoveryTime;

            LaserShotEvent(_laserShotsCount);
        }

        public void Subscribe()
        {
            if (_inputSystem == null)
                return;

            _inputSystem.Player.LaserFire.performed += OnLaserAttack;
        }

        public void Unsubscribe()
        {
            if (_inputSystem == null)
                return;

            _inputSystem.Player.LaserFire.performed -= OnLaserAttack;
        }

        private void OnLaserAttack(InputAction.CallbackContext value)
        {
            if (_laserPosition == null || _playerTransform == null)
                return;

            if (_laserShotsCount <= 0)
                return;

            _laserShotsCount = Mathf.Max(0, _laserShotsCount - 1);

            var laserPos = _laserPosition.position;
            var results = Physics2D.RaycastAll(laserPos, _playerTransform.up, float.PositiveInfinity, _layerMask);

            foreach (var result in results)
            {
                var target = result.transform.GetComponent<IDamagable>();
                target?.TakeDamage();
            }

            LaserFireEvent(laserPos, laserPos + _playerTransform.up * Vector2.Distance(_screenBounds.max, _screenBounds.min));
            LaserShotEvent(_laserShotsCount);
        }
    }
}
