using System;
using UnityEngine;

namespace Interfaces
{
    public interface ILaserAttackable
    {
        public event Action<Vector3, Vector3> LaserFireEvent;
    }
}
