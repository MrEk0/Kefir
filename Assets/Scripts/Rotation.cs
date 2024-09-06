using Interfaces;
using UnityEngine;

public class Rotation : IGameUpdatable
{
    private float _rotationSpeed;
    private Transform _transform;
    
    public Rotation(Transform target, float rotationSpeed)
    {
        _rotationSpeed = rotationSpeed;
        _transform = target;
    }
    
    public void OnUpdate(float deltaTime)
    {
        _transform.Rotate(Vector3.forward, _rotationSpeed * deltaTime);
    }
}
