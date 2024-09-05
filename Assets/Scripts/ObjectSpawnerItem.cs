using UnityEngine;
using UnityEngine.Pool;

public abstract class ObjectSpawnerItem : MonoBehaviour
{
    [SerializeField] private LayerMask _attackMask;
    
    private IObjectPool<ObjectSpawnerItem> _objectPool;

    public IObjectPool<ObjectSpawnerItem> ObjectPool
    {
        set => _objectPool = value;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        MakeCollide(other);

        if (_attackMask == (_attackMask | (1 << other.gameObject.layer)))
        {
            _objectPool.Release(this);
        }
    }
    
    protected virtual void MakeCollide(Collision other){}
}
