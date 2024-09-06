using System;
using UnityEngine;

public interface ILaserAttackable
{
    public event Action<Vector3, Vector3> LaserFireEvent;
}
