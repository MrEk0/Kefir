using System;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolItem : MonoBehaviour
{
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private int _speed;
    
    private Bounds _screenBounds;
    private Action<Collider2D> _collisionAction;

    public IObjectPool<ObjectPoolItem> ObjectPool { protected get; set; }

    public void Init(Bounds bounds, Action<Collider2D> collisionAction)
    {
        _screenBounds = bounds;
        _collisionAction = collisionAction;
    }
    
    private void Update()
    {
        var t = transform;
        var tPos = t.position;
        
        var newPos = t.up * (_speed * Time.deltaTime);
        if (!_screenBounds.IsPointInsideRect(newPos + tPos))
        {
            ObjectPool.Release(this);
        }
        else
        {
            t.position += newPos;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_attackMask != (_attackMask | (1 << other.gameObject.layer))) 
            return;

        _collisionAction(other);

        ObjectPool.Release(this);
    }
}
