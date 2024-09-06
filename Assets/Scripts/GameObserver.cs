using System.Collections.Generic;

public class GameObserver : IServisable
{
    private readonly List<IObservable> listeners = new();
    
    public void AddListener(IObservable listener)
    {
        listener.Subscribe();
        listeners.Add(listener);
    }

    public void RemoveListener(IObservable listener)
    {
        listener.Unsubscribe();
        listeners.Remove(listener);
    }

    public void RemoveAll()
    {
        foreach (var listener in listeners)
        {
            listener.Unsubscribe();
        }

        listeners.Clear();
    }
}
