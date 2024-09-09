using System;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectTargetFollowerMovement : IGameUpdatable
{
    private readonly float _velocity;
    private readonly Bounds _bounds;

    [CanBeNull] private readonly Transform _target;
    [CanBeNull] private readonly Transform _owner;
    [CanBeNull] private readonly Action _outOfBoundariesAction;

    public ObjectTargetFollowerMovement(Transform owner, Transform target, Bounds bounds, float velocity, Action outOfBoundariesAction)
    {
        _owner = owner;
        _target = target;
        _bounds = bounds;
        _velocity = velocity;
        _outOfBoundariesAction = outOfBoundariesAction;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_target == null)
            return;

        if (_owner == null)
            return;

        var tPos = _owner.position;

        var newPos = _owner.up * (_velocity * Time.deltaTime);
        if (!_bounds.IsPointInsideRect(newPos + tPos))
            _outOfBoundariesAction?.Invoke();
        else
            _owner.position += newPos;

        var direction = (_target.position - tPos).normalized;
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        _owner.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
