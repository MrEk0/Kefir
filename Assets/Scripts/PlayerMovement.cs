using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 10f;
    [SerializeField] private float _boundaryOffset = 0.5f;
    
    public event Action<float, float, Vector2> PlayerRotateEvent = delegate { };

    private float _screenHeight;
    private float _angle;
    private Vector3 _rawInputMovement;
    private Vector3 _rawInputRotation;
    private bool _isMoving;
    private bool _isRotating;

    private void Start()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        _screenHeight = mainCamera.orthographicSize;
    }

    private void Update()
    {
        var t = transform;
        var tPos = t.position;

        if (_isMoving)
        {
            var newPos = _rawInputMovement * (_moveSpeed * Time.deltaTime);
            t.position = (newPos + tPos).y > _screenHeight - _boundaryOffset ? new Vector3(0, -_screenHeight + _boundaryOffset, 0) : newPos + tPos;
        }

        if (_isRotating)
        {
            var direction = (tPos - _rawInputRotation).normalized;
            _angle += Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg * _rotateSpeed * Time.deltaTime;
            t.rotation = Quaternion.Euler(new Vector3(0, 0, _angle));
        }

        var newTrPos = t.position;
        PlayerRotateEvent(_angle, _moveSpeed, new Vector2(newTrPos.z, newTrPos.y));
    }

    public void OnMove(InputAction.CallbackContext value)
    {
        var inputMovement = value.ReadValue<Vector2>();

        _isMoving = inputMovement.y != 0;
        _isRotating = inputMovement.x != 0;

        _rawInputMovement = inputMovement.y < 0 ? Vector3.zero : new Vector3(0, inputMovement.y, 0);
        _rawInputRotation = new Vector3(0, 0, inputMovement.x);
    }
}
