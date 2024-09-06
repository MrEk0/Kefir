using UnityEngine.Pool;
using UnityEngine;
using Interfaces;

public class ObjectSpawner : MonoBehaviour, IServisable
{
    [SerializeField] private ObjectPoolItem objectPoolItem;
    [SerializeField] private bool _collectionCheck = true;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 10000;
    
    private IObjectPool<ObjectPoolItem> _objectPool;

    public IObjectPool<ObjectPoolItem> ObjectPool => _objectPool;

    public void Init()
    {
        _objectPool = new ObjectPool<ObjectPoolItem>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, _collectionCheck, _defaultCapacity, _maxSize);
    }

    private ObjectPoolItem CreateProjectile()
    {
        var spawnerItem = Instantiate(objectPoolItem, transform);
        spawnerItem.ObjectPool = _objectPool;

        return spawnerItem;
    }
  
    private void OnReleaseToPool(ObjectPoolItem pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }
    
    private void OnGetFromPool(ObjectPoolItem pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }
   
    private void OnDestroyPooledObject(ObjectPoolItem pooledObject)
    {
        Destroy(pooledObject.gameObject);
    }
}
