using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    private IObjectPool<Bullet> _objectPool;

    public IObjectPool<Bullet> ObjectPool
    {
        set => _objectPool = value;
    }
}
