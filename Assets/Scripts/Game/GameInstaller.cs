using Configs;
using Effects;
using JetBrains.Annotations;
using Player;
using Pools;
using Spawners;
using UnityEngine;

namespace Game
{
    public class GameInstaller : MonoBehaviour
    {
        [SerializeField] private GameUpdater _gameUpdater;
        [SerializeField] private PlayerVehicle _player;
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private GameSettingsData _gameSettingsData;
        [SerializeField] private AttackerObjectPoolCreator _bulletPoolCreator;
        [SerializeField] private AttackerObjectPoolCreator _asteroidPoolCreator;
        [SerializeField] private AttackerObjectPoolCreator _smallAsteroidPoolCreator;
        [SerializeField] private TargetFollowerObjectPoolCreator _ufoPoolCreator;

        [CanBeNull] public ServiceLocator ServiceLocator { get; private set; }

        public void InstallBindings()
        {
            var tr = _player.transform;
            var bounds = new Bounds(_mainCamera.transform.position,
                new Vector3(_mainCamera.orthographicSize * _mainCamera.aspect * 2f, _mainCamera.orthographicSize * 2f,
                    0f));

            var activeObjectBounds =
                new Bounds(bounds.center, bounds.size * _gameSettingsData.EnemyExtraBoundsMultiplayer);

            ServiceLocator = new ServiceLocator();
            var inputSystem = new InputSystem();
            var gameSubscriber = new GameSubscriber();
            var gameListener = new GameListener();

            ServiceLocator.AddService(inputSystem);
            ServiceLocator.AddService(_gameSettingsData);
            ServiceLocator.AddService(gameSubscriber);
            ServiceLocator.AddService(_player);
            ServiceLocator.AddService(_gameUpdater);
            ServiceLocator.AddService(gameListener);

            _asteroidPoolCreator.Init(activeObjectBounds, ServiceLocator);
            _smallAsteroidPoolCreator.Init(activeObjectBounds, ServiceLocator);
            _bulletPoolCreator.Init(activeObjectBounds, ServiceLocator);
            _ufoPoolCreator.Init(activeObjectBounds, _player.transform, ServiceLocator);

            var asteroidSpawner = new AsteroidSpawner(ServiceLocator, _asteroidPoolCreator, bounds);
            ServiceLocator.AddService(asteroidSpawner);
            _gameUpdater.AddListener(asteroidSpawner);

            var smallAsteroidSpawner = new SmallAsteroidSpawner(ServiceLocator, _smallAsteroidPoolCreator);
            ServiceLocator.AddService(smallAsteroidSpawner);
            gameSubscriber.AddListener(smallAsteroidSpawner);

            var ufoSpawner = new UFOSpawner(ServiceLocator, _ufoPoolCreator, bounds);
            ServiceLocator.AddService(ufoSpawner);
            _gameUpdater.AddListener(ufoSpawner);

            var playerAttack = new PlayerAttack(ServiceLocator, _bulletPoolCreator, _player.FirePositions, tr);
            gameSubscriber.AddListener(playerAttack);

            var playerLaserAttack = new PlayerLaserAttack(ServiceLocator, bounds, _player.LaserPosition, tr);
            ServiceLocator.AddService(playerLaserAttack);
            _gameUpdater.AddListener(playerLaserAttack);
            gameSubscriber.AddListener(playerLaserAttack);

            var playerMovement = new PlayerMovement(_mainCamera, tr, ServiceLocator);
            ServiceLocator.AddService(playerMovement);
            _gameUpdater.AddListener(playerMovement);
            gameSubscriber.AddListener(playerMovement);

            var playerLineRenderer = new LineRendererActivator(ServiceLocator, playerLaserAttack, _player.LineRenderer);
            _gameUpdater.AddListener(playerLineRenderer);
            gameSubscriber.AddListener(playerLineRenderer);

            var pointsController = new PointsController(ServiceLocator);
            gameSubscriber.AddListener(pointsController);
            ServiceLocator.AddService(pointsController);

            gameListener.AddListener(_gameUpdater);
            gameListener.AddListener(inputSystem);
            gameListener.AddListener(_asteroidPoolCreator);
            gameListener.AddListener(_smallAsteroidPoolCreator);
            gameListener.AddListener(_bulletPoolCreator);
            gameListener.AddListener(_ufoPoolCreator);
            gameListener.AddListener(playerMovement);
        }

        public void ClearBindings()
        {
            if (ServiceLocator == null)
                return;

            ServiceLocator.GetService<GameSubscriber>().RemoveAll();
            ServiceLocator.GetService<GameListener>().RemoveAll();
            _gameUpdater.RemoveAll();
            ServiceLocator.RemoveAll();
        }
    }
}
