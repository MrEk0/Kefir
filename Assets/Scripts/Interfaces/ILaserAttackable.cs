using System;
using UnityEngine;

namespace Interfaces
{
    public interface ILaserAttackable
    {
        public event Action<Vector3, Vector3> LaserFireEvent;
        public event Action<float> LaserTimerEvent;
        public event Action<int> LaserShotEvent;
    }
}
