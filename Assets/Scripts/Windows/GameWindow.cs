using TMPro;
using UnityEngine;
using Interfaces;

namespace UI
{
    public class GameWindow : MonoBehaviour, IObservable
    {
        private const float CIRCLE_ANGLE = 360f;
        
        [SerializeField] private TMP_Text _pointsText;
        [SerializeField] private TMP_Text _coordinatesText;
        [SerializeField] private TMP_Text _rotationText;
        [SerializeField] private TMP_Text _velocityText;
        [SerializeField] private TMP_Text _laserShotsText;
        [SerializeField] private TMP_Text _laserCountdownText;

        private PlayerMovement _playerMovement;
        private PlayerAttack _playerAttack;
        private SpawnSystem _spawnSystem;

        private int _currentPoints;
       
        public void Init(ServiceLocator serviceLocator, PlayerMovement playerMovement, PlayerAttack playerAttack)
        {
            _playerMovement = playerMovement;
            _playerAttack = playerAttack;
            _spawnSystem = serviceLocator.GetService<SpawnSystem>();

            _pointsText.text = _currentPoints.ToString();
        }
        
        private void OnPlayerMoved(float angle, float velocity, Vector2 coordinates)
        {
            var delta = -angle % CIRCLE_ANGLE;
            _rotationText.text = string.Format($"{(delta > 0f ? delta : Mathf.Abs(delta + CIRCLE_ANGLE)):F1}");
            _velocityText.text = string.Format($"{velocity:F1}");
            _coordinatesText.text = string.Format($"{coordinates.x:F1} : {coordinates.y:F1}");
        }

        private void OnLaserShot(Vector3 startPos, Vector3 endPos)
        {
            
        }

        private void OnEnemyKilled(int points)
        {
            _currentPoints += points;
            _pointsText.text = _currentPoints.ToString();
        }

        public void Subscribe()
        {
            _playerMovement.PlayerMoveEvent += OnPlayerMoved;
            _playerAttack.LaserFireEvent += OnLaserShot;
            _spawnSystem.ObjectKilledEvent += OnEnemyKilled;
        }

        public void Unsubscribe()
        {
            _playerMovement.PlayerMoveEvent -= OnPlayerMoved;
            _playerAttack.LaserFireEvent -= OnLaserShot;
            _spawnSystem.ObjectKilledEvent -= OnEnemyKilled;
        }
    }
}