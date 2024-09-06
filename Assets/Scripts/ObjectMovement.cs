using System;
using Interfaces;
using JetBrains.Annotations;
using UnityEngine;

public class ObjectMovement : IGameUpdatable
{
    private readonly float _velocity;
    private readonly Bounds _bounds;
    [CanBeNull]
    private readonly Transform _target;
    [CanBeNull]
    private readonly Action _outOfBoundariesAction;

    public ObjectMovement(Transform target, Bounds bounds, float velocity, Action outOfBoundariesAction)
    {
        _target = target;
        _bounds = bounds;
        _velocity = velocity;
        _outOfBoundariesAction = outOfBoundariesAction;
    }

    public void OnUpdate(float deltaTime)
    {
        if (_target == null)
            return;

        var tPos = _target.position;

        var newPos = _target.up * (_velocity * Time.deltaTime);
        if (!_bounds.IsPointInsideRect(newPos + tPos))
            _outOfBoundariesAction?.Invoke();
        else
            _target.position += newPos;
    }
}
