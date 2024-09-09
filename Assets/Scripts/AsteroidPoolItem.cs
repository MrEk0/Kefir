using System;
using Interfaces;
using JetBrains.Annotations;

public class AsteroidPoolItem : AttackablePoolItem, IDamagable
{
    [CanBeNull]
    private Action<ObjectPoolItem> _takeDamageAction;

    private bool _isDamaged;

    public void Init(Action<ObjectPoolItem> action)
    {
        _takeDamageAction = action;
    }
    
    public void TakeDamage()
    {
        if (_isDamaged)
            return;

        _isDamaged = true;
        _takeDamageAction?.Invoke(this);
    }
}
