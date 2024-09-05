using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(menuName = "Kefir/Configs/GameSettingsData")]
    public class GameSettingsData : ScriptableObject
    {
        [SerializeField] private int _playerVelocity = 0;
        [SerializeField] private int _playerAngleVelocity = 0;
        [SerializeField] private int _bulletVelocity = 0;
        [SerializeField] private int _maxLaserShotCount = 0;
        [SerializeField] private int _asteroidOnBulletHitCount;
        [SerializeField] private int _coinsForAsteroid;
        [SerializeField] private int _coinsForSmallAsteroid;
        [SerializeField] private int _coinsForUFO;

        public int PlayerVelocity => _playerVelocity;
        public int PlayerAngleVelocity => _playerAngleVelocity;
        public int MaxLaserShotCount => _maxLaserShotCount;
        public int AsteroidOnBulletHitCount => _asteroidOnBulletHitCount;
        public int CoinsForAsteroid => _coinsForAsteroid;
        public int CoinsForSmallAsteroid => _coinsForSmallAsteroid;
        public int CoinsForUFO => _coinsForUFO;
    }
}
