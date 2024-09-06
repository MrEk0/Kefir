using System.Collections.Generic;
using UnityEngine;
using Interfaces;

public class ServiceLocator
{
    private readonly List<IServisable> _services = new();

    public void AddService(IServisable service)
    {
        _services.Add(service);
    }

    public void RemoveService(IServisable service)
    {
        _services.Remove(service);
    }

    public void RemoveAll()
    {
        _services.Clear();
    }

    public T GetService<T>()
    {
        foreach (var service in _services)
        {
            if (service is T result)
            {
                return result;
            }
        }

        Debug.LogError($"Service of type {typeof(T)} is not found!");
        return default;
    }
}
