using JetBrains.Annotations;
using Interfaces;

public class InputSystem : IServisable
{
    [CanBeNull]
    private InputSystemActions _inputSystemActions { get; }

    public InputSystemActions.PlayerActions Player => _inputSystemActions?.Player ?? new InputSystemActions.PlayerActions();

    public InputSystem()
    {
        _inputSystemActions = new InputSystemActions();
        _inputSystemActions.Player.Enable();
    }
}
