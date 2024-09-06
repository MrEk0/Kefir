using System;
using Configs;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : IGameUpdatable, IServisable, IObservable
{
    public event Action<float, float, Vector2> PlayerRotateEvent = delegate { };

    private float _screenHeight;
    private float _angle;
    private float _boundaryOffset = 0.5f;
    private Vector3 _rawInputMovement;
    private Vector3 _rawInputRotation;
    private bool _isMoving;
    private bool _isRotating;
    private InputSystem _inputSystem;
    private Transform _transform;
    private GameSettingsData _gameSettingsData;

    public PlayerMovement(Camera mainCamera, Transform transform, ServiceLocator serviceLocator)
    {
        _screenHeight = mainCamera.orthographicSize;
        _inputSystem = serviceLocator.GetService<InputSystem>();
        _transform = transform;
        _gameSettingsData = serviceLocator.GetService<GameSettingsData>();
    }
    
    public void Subscribe()
    {
        _inputSystem.Player.Move.started += OnMoveStarted;
        _inputSystem.Player.Move.canceled += OnMoveCanceled;
    }

    public void Unsubscribe()
    {
        _inputSystem.Player.Move.started -= OnMoveStarted;
        _inputSystem.Player.Move.canceled += OnMoveCanceled;
    }
    
    public void OnUpdate(float deltaTime)
    {
        var t = _transform;
        var tPos = t.position;

        if (_isMoving)
        {
            var newPos = _rawInputMovement * (_gameSettingsData.PlayerVelocity * deltaTime);
            t.position = (newPos + tPos).y > _screenHeight - _boundaryOffset ? new Vector3(0, -_screenHeight + _boundaryOffset, 0) : newPos + tPos;
        }

        if (_isRotating)
        {
            var direction = (tPos - _rawInputRotation).normalized;
            _angle += Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg * _gameSettingsData.PlayerAngleVelocity * deltaTime;
            t.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));
        }

        var newTrPos = t.position;
        PlayerRotateEvent(_angle, _gameSettingsData.PlayerVelocity, new Vector2(newTrPos.z, newTrPos.y));
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
}
