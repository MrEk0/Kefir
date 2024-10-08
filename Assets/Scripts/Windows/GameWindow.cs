using System;
using Game;
using Player;
using TMPro;
using UnityEngine;

namespace Windows
{
    public class GameWindow : AWindow<GameWindowSetup>, IDisposable
    {
        private const float CIRCLE_ANGLE = 360f;
        
        [SerializeField] private TMP_Text _pointsText;
        [SerializeField] private TMP_Text _coordinatesText;
        [SerializeField] private TMP_Text _rotationText;
        [SerializeField] private TMP_Text _velocityText;
        [SerializeField] private TMP_Text _laserShotsText;
        [SerializeField] private TMP_Text _laserTimerText;

        private PlayerMovement _playerMovement;
        private PlayerLaserAttack _playerAttack;
        private PointsController _pointController;

        public override void Init(GameWindowSetup windowSetup)
        {
            _playerMovement = windowSetup.ServiceLocator.GetService<PlayerMovement>();
            _playerAttack = windowSetup.ServiceLocator.GetService<PlayerLaserAttack>();
            _pointController = windowSetup.ServiceLocator.GetService<PointsController>();

            _playerMovement.PlayerMoveEvent += OnPlayerMoved;
            _playerAttack.LaserShotEvent += OnLaserShot;
            _playerAttack.LaserTimerEvent += OnLaserTimerUpdate;
            _pointController.PointsUpdateEvent += OnPointsUpdated;
        }
        
        public void Dispose()
        {
            _playerMovement.PlayerMoveEvent -= OnPlayerMoved;
            _playerAttack.LaserShotEvent -= OnLaserShot;
            _playerAttack.LaserTimerEvent -= OnLaserTimerUpdate;
            _pointController.PointsUpdateEvent -= OnPointsUpdated;
        }
        
        private void OnPlayerMoved(float angle, float velocity, Vector2 coordinates)
        {
            var delta = -angle % CIRCLE_ANGLE;
            _rotationText.text = string.Format($"{(delta > 0f ? delta : Mathf.Abs(delta + CIRCLE_ANGLE)):F1}");
            _velocityText.text = string.Format($"{velocity:F1}");
            _coordinatesText.text = string.Format($"{coordinates.x:F1} : {coordinates.y:F1}");
        }

        private void OnLaserTimerUpdate(float timer)
        {
            _laserTimerText.text = string.Format($"{timer:F1}");
        }

        private void OnLaserShot(int shotsCount)
        {
            _laserShotsText.text = shotsCount.ToString();
        }

        private void OnPointsUpdated(int points)
        {
            _pointsText.text = points.ToString();
        }
    }
}