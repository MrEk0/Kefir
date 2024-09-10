using JetBrains.Annotations;
using Interfaces;

public class InputSystem : IServisable, IGameStartable, IGameFinishable
{
    [CanBeNull]
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
