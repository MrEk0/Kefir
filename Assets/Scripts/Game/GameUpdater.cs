using System.Collections.Generic;
using Interfaces;
using UnityEngine;

namespace Game
{
    public class GameUpdater : MonoBehaviour, IGameStartListener, IGameFinishListener
    {
        private readonly List<IGameUpdatable> _updateListeners = new();

        private bool _isActive;

        public void AddListener(IGameUpdatable listener)
        {
            _updateListeners.Add(listener);
        }

        public void RemoveListener(IGameUpdatable listener)
        {
            _updateListeners.Remove(listener);
        }

        public void RemoveAll()
        {
            _updateListeners.Clear();
        }

        public void StartGame()
        {
            _isActive = true;
        }

        public void GameFinish()
        {
            _isActive = false;
        }

        private void Update()
        {
            if (!_isActive)
                return;

            var deltaTime = Time.deltaTime;
            for (var i = 0; i < _updateListeners.Count; i++)
            {
                _updateListeners[i].OnUpdate(deltaTime);
            }
        }
    }
}
