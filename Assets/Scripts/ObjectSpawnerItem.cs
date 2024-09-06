using UnityEngine;
using UnityEngine.Pool;

public abstract class ObjectSpawnerItem : MonoBehaviour
{
    public IObjectPool<ObjectSpawnerItem> ObjectPool { set; protected get; }
}
