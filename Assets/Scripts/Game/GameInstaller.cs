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
        [SerializeField] private AttackerObjectPoolFactory bulletPoolFactory;
        [SerializeField] private DamageReceiverFactory asteroidFactory;
        [SerializeField] private DamageReceiverFactory smallAsteroidFactory;
        [SerializeField] private TargetFollowerObjectFactory ufoFactory;

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
            var gameListener = new GameListener();

            ServiceLocator.AddService(inputSystem);
            ServiceLocator.AddService(_gameSettingsData);
            ServiceLocator.AddService(_player);
            ServiceLocator.AddService(_gameUpdater);
            ServiceLocator.AddService(gameListener);

            asteroidFactory.Init(activeObjectBounds, ServiceLocator);
            smallAsteroidFactory.Init(activeObjectBounds, ServiceLocator);
            bulletPoolFactory.Init(activeObjectBounds, ServiceLocator);
            ufoFactory.Init(activeObjectBounds, _player.transform, ServiceLocator);

            var asteroidSpawner = new AsteroidSpawner(ServiceLocator, asteroidFactory, bounds);
            ServiceLocator.AddService(asteroidSpawner);
            _gameUpdater.AddListener(asteroidSpawner);

            var smallAsteroidSpawner = new SmallAsteroidSpawner(ServiceLocator, smallAsteroidFactory);
            ServiceLocator.AddService(smallAsteroidSpawner);

            var ufoSpawner = new UFOSpawner(ServiceLocator, ufoFactory, bounds);
            ServiceLocator.AddService(ufoSpawner);
            _gameUpdater.AddListener(ufoSpawner);

            var playerAttack = new PlayerAttack(ServiceLocator, bulletPoolFactory, _player.FirePositions, tr);
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
            gameListener.AddListener(asteroidFactory);
            gameListener.AddListener(smallAsteroidFactory);
            gameListener.AddListener(bulletPoolFactory);
            gameListener.AddListener(ufoFactory);
            gameListener.AddListener(playerMovement);
        }

        public void ClearBindings()
        {
            if (ServiceLocator == null)
                return;
            
            ServiceLocator.GetService<GameListener>().RemoveAll();
            _gameUpdater.RemoveAll();
            ServiceLocator.RemoveAll();
        }
    }
}
