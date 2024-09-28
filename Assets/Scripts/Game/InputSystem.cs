using JetBrains.Annotations;
using Interfaces;

namespace Game
{
    public class InputSystem : IGameStartListener, IGameFinishListener
    {
        private InputSystemActions _inputSystemActions { get; }

        public InputSystemActions.PlayerActions Player => _inputSystemActions?.Player ?? new InputSystemActions.PlayerActions();

        public InputSystem()
        {
            _inputSystemActions = new InputSystemActions();
        }

        public void StartGame()
        {
            Player.Enable();
        }

        public void GameFinish()
        {
            Player.Disable();
        }
    }
}
