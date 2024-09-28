using System;
using Configs;
using Game;
using UnityEngine;
using UnityEngine.InputSystem;
using Interfaces;
using InputSystem = Game.InputSystem;

namespace Player
{
    public class PlayerMovement : IGameUpdatable, IDisposable, IGameStartListener
    {
        public event Action<float, float, Vector2> PlayerMoveEvent = delegate { };

        private readonly InputSystem _inputSystem;
        private readonly Transform _transform;

        private readonly float _screenHeight;
        private readonly float _boundaryOffset;
        private readonly float _angleVelocity;
        private readonly float _maxVelocity;
        private readonly float _velocityTimeRate;

        private Vector3 _rawInputMovement;
        private Vector3 _rawInputRotation;
        private bool _isMoving;
        private bool _isRotating;
        private float _angle;
        private float _currentVelocity;

        public PlayerMovement(Camera mainCamera, Transform transform, ServiceLocator serviceLocator)
        {
            _screenHeight = mainCamera.orthographicSize;
            _inputSystem = serviceLocator.GetService<InputSystem>();
            _transform = transform;

            var data = serviceLocator.GetService<GameSettingsData>();
            _maxVelocity = data.PlayerMaxVelocity;
            _velocityTimeRate = data.PlayerVelocityTimeRate;
            _angleVelocity = data.PlayerAngleVelocity;

            _inputSystem.Player.Move.started += OnMoveStarted;
            _inputSystem.Player.Move.canceled += OnMoveCanceled;
        }

        public void Dispose()
        {
            _inputSystem.Player.Move.started -= OnMoveStarted;
            _inputSystem.Player.Move.canceled -= OnMoveCanceled;
        }

        public void OnUpdate(float deltaTime)
        {
            var tPos = _transform.position;

            _currentVelocity = _isMoving
                ? Mathf.Min(_maxVelocity, _currentVelocity + _velocityTimeRate * deltaTime)
                : Mathf.Max(0f, _currentVelocity - _velocityTimeRate * deltaTime);

            var newPos = _rawInputMovement * (_currentVelocity * deltaTime);
            _transform.position = (newPos + tPos).y > _screenHeight - _boundaryOffset
                ? new Vector3(0, -_screenHeight + _boundaryOffset, 0)
                : newPos + tPos;

            if (_isRotating)
            {
                var direction = (tPos - _rawInputRotation).normalized;
                _angle += Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg * _angleVelocity * deltaTime;
                _transform.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));
            }

            var newTrPos = _transform.position;
            PlayerMoveEvent(_angle, _currentVelocity, new Vector2(newTrPos.z, newTrPos.y));
        }

        private void OnMoveCanceled(InputAction.CallbackContext value)
        {
            _isMoving = false;
            _isRotating = false;
        }

        private void OnMoveStarted(InputAction.CallbackContext value)
        {
            var inputMovement = value.ReadValue<Vector2>();

            _isMoving = inputMovement.y != 0;
            _isRotating = inputMovement.x != 0;

            _rawInputMovement = inputMovement.y < 0 ? Vector3.zero : new Vector3(0, inputMovement.y, 0);
            _rawInputRotation = new Vector3(0, 0, inputMovement.x);
        }

        public void StartGame()
        {
            _transform.position = Vector3.zero;
            _transform.rotation = Quaternion.identity;
        }
    }
}
