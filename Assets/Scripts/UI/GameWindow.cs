using TMPro;
using UnityEngine;

namespace UI
{
    public class GameWindow : MonoBehaviour
    {
        private const float CIRCLE_ANGLE = 360f;
        
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private TMP_Text _pointsText;
        [SerializeField] private TMP_Text _coordinatesText;
        [SerializeField] private TMP_Text _rotationText;
        [SerializeField] private TMP_Text _velocityText;
        [SerializeField] private TMP_Text _laserShotsText;
        [SerializeField] private TMP_Text _laserCountdownText;

        private void OnEnable()
        {
            _playerMovement.PlayerRotateEvent += OnPlayerRotated;
        }

        private void OnDisable()
        {

        }

        private void OnPlayerRotated(float angle, float velocity, Vector2 coordinates)
        {
            var delta = -angle % CIRCLE_ANGLE;
            _rotationText.text = string.Format($"{(delta > 0f ? delta : Mathf.Abs(delta + CIRCLE_ANGLE)):F1}");
            _velocityText.text = string.Format($"{velocity:F1}");
            _coordinatesText.text = string.Format($"{coordinates.x:F1} : {coordinates.y:F1}");
        }

        private void OnLaserShot()
        {

        }
    }
}