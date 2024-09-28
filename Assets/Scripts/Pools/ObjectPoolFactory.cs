using UnityEngine;
using Interfaces;

namespace Pools
{
    public abstract class ObjectPoolFactory : MonoBehaviour
    {
        public static bool CanRelease(ObjectPoolItem poolObject) => poolObject.gameObject.activeSelf;

        protected static void OnReleaseToPool(ObjectPoolItem pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }

        protected static void OnGetFromPool(ObjectPoolItem pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
        }
    }
}
