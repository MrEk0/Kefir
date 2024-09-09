using System;
using Interfaces;
using JetBrains.Annotations;

public class DamageReceiverPoolItem : AttackerPoolItem, IDamagable
{
    [CanBeNull]
    private Action<ObjectPoolItem> _takeDamageAction;

    public void Init(Action<ObjectPoolItem> action)
    {
        _takeDamageAction = action;
    }
    
    public void TakeDamage()
    {
        _takeDamageAction?.Invoke(this);
    }
}
