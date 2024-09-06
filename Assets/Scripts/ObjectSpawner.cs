using UnityEngine.Pool;
using UnityEngine;

public abstract class ObjectSpawner : MonoBehaviour, IServisable
{
    [SerializeField] private ObjectSpawnerItem _objectSpawnerItem;
    [SerializeField] private bool _collectionCheck = true;
    [SerializeField] private int _defaultCapacity = 10;
    [SerializeField] private int _maxSize = 10000;
    
    private IObjectPool<ObjectSpawnerItem> _objectPool;

    public IObjectPool<ObjectSpawnerItem> ObjectPool => _objectPool;

    public void Init()
    {
        _objectPool = new ObjectPool<ObjectSpawnerItem>(CreateProjectile, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject, _collectionCheck, _defaultCapacity, _maxSize);
    }

    protected virtual void MakeDestroy(ObjectSpawnerItem item) { }
    protected virtual void MakeCreate(ObjectSpawnerItem item) { }

    private ObjectSpawnerItem CreateProjectile()
    {
        var spawnerItem = Instantiate(_objectSpawnerItem, transform);
        spawnerItem.ObjectPool = _objectPool;
        
        MakeCreate(spawnerItem);
        
        return spawnerItem;
    }
  
    private void OnReleaseToPool(ObjectSpawnerItem pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }
    
    private void OnGetFromPool(ObjectSpawnerItem pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }
   
    private void OnDestroyPooledObject(ObjectSpawnerItem pooledObject)
    {
        MakeDestroy(pooledObject);
        
        Destroy(pooledObject.gameObject);
    }
}
