using UnityEngine;

public class BulletSpawnerItem : ObjectSpawnerItem
{
    [SerializeField] private LayerMask _attackMask;
    [SerializeField] private int _speed = 15;
    
    private float _screenHeight;
    private Bounds _screenBounds;
    
    private void Start()
    {
        var mainCamera = Camera.main;
        if (mainCamera == null)
            return;

        _screenHeight = mainCamera.orthographicSize;
        _screenBounds = new Bounds(Vector3.zero, new Vector3(0, _screenHeight * 2, _screenHeight * mainCamera.aspect));
    }

    private void Update()
    {
        var t = transform;
        var tPos = t.position;
        
        var newPos = t.up * (_speed * Time.deltaTime);
        if ((newPos + tPos).y > _screenHeight || (newPos + tPos).y > _screenHeight)
        {
            ObjectPool.Release(this);
        }
        else
        {
            t.position += newPos;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (_attackMask == (_attackMask | (1 << other.gameObject.layer)))
        {
            //todo hit
            
            ObjectPool.Release(this);
        }
    }
}
