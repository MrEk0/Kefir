using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Windows
{
    public class GameOverWindow : AWindow<GameOverWindowSetup>
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Button _replayButton;

        [CanBeNull]
        private WindowSystem _windowSystem;

        private void OnEnable()
        {
            _replayButton.onClick.AddListener(OnReplayClicked);
        }

        private void OnDisable()
        {
            _replayButton.onClick.RemoveListener(OnReplayClicked);
        }

        public override void Init(GameOverWindowSetup windowSetup)
        {
            _windowSystem = windowSetup.WindowSystem;
            _scoreText.text = windowSetup.Score.ToString();
        }

        private void OnReplayClicked()
        {
            if (_windowSystem == null)
                return;
            
            //todo replay event
            _windowSystem.Close<GameOverWindow>();
        }
    }
}
