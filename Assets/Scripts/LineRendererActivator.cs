using Configs;
using UnityEngine;

public class LineRendererActivator : IObservable, IGameUpdatable
{
    private readonly LineRenderer _lineRenderer;
    private readonly ILaserAttackable _attacker;
    private readonly float _duration;
    
    private float _timer;
    private bool _isActive;
    
    public LineRendererActivator(ServiceLocator serviceLocator, ILaserAttackable attacker, LineRenderer lineRenderer)
    {
        _lineRenderer = lineRenderer;
        _attacker = attacker;

        _isActive = false;
        _lineRenderer.enabled = _isActive;
        _duration = serviceLocator.GetService<GameSettingsData>().LaserDuration;
    }

    public void Subscribe()
    {
        _attacker.LaserFireEvent += OnLaserAttack;
    }

    public void Unsubscribe()
    {
        _attacker.LaserFireEvent -= OnLaserAttack;
    }

    public void OnUpdate(float deltaTime)
    {
        if (!_isActive)
            return;
        
        _timer += deltaTime;

        if (_timer < _duration)
            return;
        
        _isActive = false;
        _timer = 0f;

        _lineRenderer.enabled = _isActive;
    }
    
    private void OnLaserAttack(Vector3 initPosition, Vector3 finishPosition)
    {
        _isActive = true;
        
        _lineRenderer.enabled = _isActive;
        
        _lineRenderer.SetPosition(0, initPosition);
        _lineRenderer.SetPosition(1, initPosition + finishPosition);
    }
}
