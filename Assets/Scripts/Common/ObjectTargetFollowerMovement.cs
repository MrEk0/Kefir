using System;
using Extensions;
using Interfaces;
using UnityEngine;

namespace Common
{
    public class ObjectTargetFollowerMovement : IGameUpdatable
    {
        private readonly float _velocity;
        private readonly Bounds _bounds;
        private readonly Transform _target;
        private readonly Transform _owner;
        private readonly Action _outOfBoundariesAction;

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
}
