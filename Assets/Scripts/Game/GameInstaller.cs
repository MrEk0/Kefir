using Configs;
using Effects;
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
        [SerializeField] private AttackerObjectPoolFactory _bulletPoolFactory;
        [SerializeField] private DamageReceiverFactory _asteroidFactory;
        [SerializeField] private DamageReceiverFactory _smallAsteroidFactory;
        [SerializeField] private TargetFollowerObjectFactory _ufoFactory;

        public ServiceLocator ServiceLocator { get; private set; }

        public void InstallBindings()
        {
            var tr = _player.transform;
            var bounds = new Bounds(_mainCamera.transform.position, new Vector3(_mainCamera.orthographicSize * _mainCamera.aspect * 2f, _mainCamera.orthographicSize * 2f, 0f));

            var activeObjectBounds = new Bounds(bounds.center, bounds.size * _gameSettingsData.EnemyExtraBoundsMultiplayer);

            ServiceLocator = new ServiceLocator();
            var inputSystem = new InputSystem();
            var gameListener = new GameListener();

            ServiceLocator.AddService(inputSystem);
            ServiceLocator.AddService(_gameSettingsData);
            ServiceLocator.AddService(_player);
            ServiceLocator.AddService(_gameUpdater);
            ServiceLocator.AddService(gameListener);

            _asteroidFactory.Init(activeObjectBounds, ServiceLocator);
            _smallAsteroidFactory.Init(activeObjectBounds, ServiceLocator);
            _bulletPoolFactory.Init(activeObjectBounds, ServiceLocator);
            _ufoFactory.Init(activeObjectBounds, _player.transform, ServiceLocator);

            var asteroidSpawner = new AsteroidSpawner(ServiceLocator, _asteroidFactory, bounds);
            ServiceLocator.AddService(asteroidSpawner);
            _gameUpdater.AddListener(asteroidSpawner);

            var smallAsteroidSpawner = new SmallAsteroidSpawner(ServiceLocator, _smallAsteroidFactory);
            ServiceLocator.AddService(smallAsteroidSpawner);

            var ufoSpawner = new UFOSpawner(ServiceLocator, _ufoFactory, bounds);
            ServiceLocator.AddService(ufoSpawner);
            _gameUpdater.AddListener(ufoSpawner);

            var playerAttack = new PlayerAttack(ServiceLocator, _bulletPoolFactory, _player.FirePositions, tr);
            var playerLaserAttack = new PlayerLaserAttack(ServiceLocator, bounds, _player.LaserPosition, tr);
            ServiceLocator.AddService(playerLaserAttack);
            _gameUpdater.AddListener(playerLaserAttack);

            var playerMovement = new PlayerMovement(_mainCamera, tr, ServiceLocator);
            ServiceLocator.AddService(playerMovement);
            _gameUpdater.AddListener(playerMovement);
            
            var playerLineRenderer = new LineRendererActivator(ServiceLocator, playerLaserAttack, _player.LineRenderer);
            _gameUpdater.AddListener(playerLineRenderer);

            var pointsController = new PointsController(ServiceLocator);
            ServiceLocator.AddService(pointsController);

            gameListener.AddListener(_gameUpdater);
            gameListener.AddListener(inputSystem);
            gameListener.AddListener(_asteroidFactory);
            gameListener.AddListener(_smallAsteroidFactory);
            gameListener.AddListener(_bulletPoolFactory);
            gameListener.AddListener(_ufoFactory);
            gameListener.AddListener(playerMovement);
        }

        public void ClearBindings()
        {
            ServiceLocator.GetService<GameListener>().RemoveAll();
            _gameUpdater.RemoveAll();
            ServiceLocator.RemoveAll();
        }
    }
}
