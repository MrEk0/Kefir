using System.Collections.Generic;
using Interfaces;

public class GameObserver : IServisable
{
    private readonly List<IObservable> _listeners = new();
    
    public void AddListener(IObservable listener)
    {
        listener.Subscribe();
        _listeners.Add(listener);
    }

    public void RemoveListener(IObservable listener)
    {
        listener.Unsubscribe();
        _listeners.Remove(listener);
    }

    public void RemoveAll()
    {
        foreach (var listener in _listeners)
        {
            listener.Unsubscribe();
        }

        _listeners.Clear();
    }
}
