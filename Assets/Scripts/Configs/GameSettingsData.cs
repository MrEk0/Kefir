using UnityEngine;
using Interfaces;

namespace Configs
{
    [CreateAssetMenu(menuName = "Kefir/Configs/GameSettingsData")]
    public class GameSettingsData : ScriptableObject, IServisable
    {
        [SerializeField] private int _playerVelocity = 0;
        [SerializeField] private int _playerAngleVelocity = 0;
        [SerializeField] private int _bulletVelocity = 0;
        [SerializeField] private int _maxLaserShotCount = 0;
        [SerializeField] private float _laserDuration;
        [SerializeField] private float _asteroidSpawnPause;
        [SerializeField] private int _asteroidOnBulletHitCount;
        [SerializeField] private int _coinsForAsteroid;
        [SerializeField] private int _coinsForSmallAsteroid;
        [SerializeField] private int _coinsForUFO;
        [SerializeField] private Vector2 _asteroidTargetAngleRange;
        [SerializeField] private LayerMask _enemyAttackMask;
        [SerializeField] private LayerMask _playerAttackMask;

        public int PlayerVelocity => _playerVelocity;
        public int PlayerAngleVelocity => _playerAngleVelocity;
        public int MaxLaserShotCount => _maxLaserShotCount;
        public float LaserDuration => _laserDuration;
        public float AsteroidSpawnPause => _asteroidSpawnPause;
        public int AsteroidOnBulletHitCount => _asteroidOnBulletHitCount;
        public int CoinsForAsteroid => _coinsForAsteroid;
        public int CoinsForSmallAsteroid => _coinsForSmallAsteroid;
        public int CoinsForUFO => _coinsForUFO;
        public Vector2 AsteroidTargetAngleRange => _asteroidTargetAngleRange;
        public LayerMask EnemyAttackMask => _enemyAttackMask;
    }
}
