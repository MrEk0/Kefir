using UnityEngine;
using Interfaces;
using UnityEngine.Pool;

public abstract class ObjectPoolCreator : MonoBehaviour, IServisable
{
    public IObjectPool<ObjectPoolItem> ObjectPool { get; protected set; }
    
    public bool CanRelease(ObjectPoolItem poolObject) => poolObject.gameObject.activeSelf;

    protected abstract ObjectPoolItem CreateProjectile();
    protected abstract void OnDestroyPooledObject(ObjectPoolItem pooledObject);

    protected static void OnReleaseToPool(ObjectPoolItem pooledObject)
    {
        pooledObject.gameObject.SetActive(false);
    }

    protected static void OnGetFromPool(ObjectPoolItem pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
    }
}
