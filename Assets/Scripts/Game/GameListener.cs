using System.Collections.Generic;
using Interfaces;

namespace Game
{
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
                if (listener is IGameStartListener gameStartListener)
                {
                    gameStartListener.StartGame();
                }
            }
        }

        public void FinishGame()
        {
            foreach (var listener in _listeners)
            {
                if (listener is IGameFinishListener gameFinishListener)
                {
                    gameFinishListener.GameFinish();
                }
            }
        }

        public void RemoveAll()
        {
            _listeners.Clear();
        }
    }
}
