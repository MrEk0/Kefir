using System.Collections.Generic;
using Interfaces;

public class GameListener : IServisable
{
    private readonly List<object> _listeners = new();
    
    public void AddListener(object listener)
    {
        _listeners.Add(listener);
    }

    public void RemoveListener(object listener)
    {
        _listeners.Remove(listener);
    }

    public void StartGame()
    {
        foreach (var listener in _listeners)
        {
            if (listener is IGameStartable gameStartable)
            {
                gameStartable.StartGame();
            }
        }
    }
    
    public void FinishGame()
    {
        foreach (var listener in _listeners)
        {
            if (listener is IGameFinishable gameFinishable)
            {
                gameFinishable.GameFinish();
            }
        }
    }

    public void RemoveAll()
    {
        _listeners.Clear();
    }
}
