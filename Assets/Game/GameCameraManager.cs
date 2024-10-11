using Umeshu.Uf;
using Umeshu.USystem.Time;
using UnityEngine;

namespace Umeshu.USystem.GameCameraManager
{
    public sealed class GameCameraManager : SceneSystem<GameCameraManager>
    {
        [Header("Scene Links")]
        [SerializeField] private Transform shakeRoot;
        [SerializeField] private Camera cam;

        private Ball ball;
        private Level_GameZone level_GameZone;

        public const float WANTED_CAM_SIZE_X = 16f;
        public const float WANTED_CAM_SIZE_Y = 24f;
        public const float WANTED_CAM_SIZE_Z = -10f;

        public static Camera Camera => Instance.cam;

        public void SetBallAndGameZone(Ball _ball, Level_GameZone _level_GameZone)
        {
            ball = _ball;
            level_GameZone = _level_GameZone;
        }
        public Vector2 GetClosestPositionInScreen(Vector2 _position) => UfMath.LineIntersectionOnRect(new Vector2(cam.GetHorizontalCamSize(), cam.GetVerticalCamSize()), cam.transform.position, _position);

        protected override TimeThread GetThread() => TimeThread.General;
        protected override void SystemFirstInitialize() { }
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate()
        {
            cam.orthographic = true;

            if (level_GameZone != null)
            {
                float _camSizeX = cam.GetHorizontalCamSize();
                float _camSizeY = cam.GetVerticalCamSize();

                Vector2 _maxSizeFromGameZone = level_GameZone.transform.localScale;
                Vector2 _gameZonePosition = level_GameZone.transform.position;

                float _wantedSizeX = Mathf.Min(_maxSizeFromGameZone.x, WANTED_CAM_SIZE_X);
                float _wantedSizeY = Mathf.Min(_maxSizeFromGameZone.y, WANTED_CAM_SIZE_Y);

                float _ratioToMultiplySizeX = _wantedSizeX / _camSizeX;
                float _ratioToMultiplySizeY = _wantedSizeY / _camSizeY;

                float _ratioToMultiplySize = Mathf.Max(_ratioToMultiplySizeX, _ratioToMultiplySizeY);

                cam.orthographicSize *= _ratioToMultiplySize;

                Vector2 _ballPos = ball.transform.position;

                float _seedDecalX = _ballPos.x - _gameZonePosition.x;
                float _seedDecalY = _ballPos.y - _gameZonePosition.y;

                float _spaceWithBorderToBeMax = 5;
                float _percentageIsAtMaxAtX = 1 - Mathf.Clamp01(_spaceWithBorderToBeMax * 2 / _wantedSizeX);
                float _percentageIsAtMaxAtY = 1 - Mathf.Clamp01(_spaceWithBorderToBeMax * 2 / _wantedSizeY);
                if (_percentageIsAtMaxAtX <= 0) _percentageIsAtMaxAtX = 0.001f;
                if (_percentageIsAtMaxAtY <= 0) _percentageIsAtMaxAtY = 0.001f;

                float _movementXPercentage = UfMath.EaseOut(Mathf.Abs(_seedDecalX / _maxSizeFromGameZone.x) / _percentageIsAtMaxAtX) * Mathf.Sign(_seedDecalX);
                float _movementYPercentage = UfMath.EaseOut(Mathf.Abs(_seedDecalY / _maxSizeFromGameZone.y) / _percentageIsAtMaxAtY) * Mathf.Sign(_seedDecalY);

                float _maxMovementX = Mathf.Max(0, _maxSizeFromGameZone.x - WANTED_CAM_SIZE_X) / 2f;
                float _maxMovementY = Mathf.Max(0, _maxSizeFromGameZone.y - WANTED_CAM_SIZE_Y) / 2f;

                float _movementX = _maxMovementX * _movementXPercentage;
                float _movementY = _maxMovementY * _movementYPercentage;


                Vector3 _wantedCamPos = _gameZonePosition;
                _wantedCamPos.x += _movementX;
                _wantedCamPos.y += _movementY;
                _wantedCamPos.z = WANTED_CAM_SIZE_Z;

                cam.transform.position = _wantedCamPos;
            }
            else
            {
                cam.transform.position = ball.transform.position;
            }

            cam.PlaceCameraAsPerspectiveFromOrthographicSize(_focusZPosition: 0);
        }

        public Vector4 GetCameraBoundaries()
        {
            cam.GetCameraWorldBounds(out Vector2 _cameraHorizontalBounds, out Vector2 _cameraVerticalBounds);
            return new Vector4(_cameraHorizontalBounds.x, _cameraHorizontalBounds.y, _cameraVerticalBounds.x, _cameraVerticalBounds.y);
        }
    }
}