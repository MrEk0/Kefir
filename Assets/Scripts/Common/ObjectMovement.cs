using System;
using Extensions;
using Interfaces;
using UnityEngine;

namespace Common
{
    public class ObjectMovement : IGameUpdatable
    {
        private readonly float _velocity;
        private readonly Bounds _bounds;
        private readonly Transform _owner;
        private readonly Action _outOfBoundariesAction;

        public ObjectMovement(Transform owner, Bounds bounds, float velocity, Action outOfBoundariesAction)
        {
            _owner = owner;
            _bounds = bounds;
            _velocity = velocity;
            _outOfBoundariesAction = outOfBoundariesAction;
        }

        public void OnUpdate(float deltaTime)
        {
            if (_owner == null)
                return;

            var tPos = _owner.position;

            var newPos = _owner.up * (_velocity * Time.deltaTime);
            if (!_bounds.IsPointInsideRect(newPos + tPos))
                _outOfBoundariesAction?.Invoke();
            else
                _owner.position += newPos;
        }
    }
}
