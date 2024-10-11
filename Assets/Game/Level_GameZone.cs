using Umeshu.Uf;
using UnityEngine;

public class Level_GameZone : HeritableGameElement
{
    [Header("Links")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void OnEnable() => spriteRenderer.enabled = false;

    private const float HIGHEST_HORIZONTAL_RATIO_MOBILE = 21.6f / 18f; // Z Fold 5
    private const float HIGHEST_HORIZONTAL_RATIO_PC = 9f / 21f; // Cinema Ratio
    private const float HIGHEST_VERTICAL_RATIO = 25f / 9f; // Samsung Galaxy Fold 2 5G

    private const float SIDE_BORDER_SIZE = 2;
    private const float TOP_AND_BOTTOM_BORDER_SIZE = 4;

    public const float WANTED_CAM_SIZE_X = 16f;
    public const float WANTED_CAM_SIZE_Y = 24f;

    private Vector3 startPosition = Vector3.zero;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector2 _gameZoneSize = transform.localScale;
        Vector2 _cameraWantedSize = new(WANTED_CAM_SIZE_X, WANTED_CAM_SIZE_Y);

        float _cameraRatio = _cameraWantedSize.y / _cameraWantedSize.x;
        float _sizeYFromGameZoneX = _gameZoneSize.x * _cameraRatio;

        float _finalSizeY = Mathf.Min(_sizeYFromGameZoneX, _gameZoneSize.y);
        float _finalSizeX = _finalSizeY / _cameraRatio;


        if (_finalSizeX > WANTED_CAM_SIZE_X) _finalSizeX = WANTED_CAM_SIZE_X;
        if (_finalSizeY > WANTED_CAM_SIZE_Y) _finalSizeY = WANTED_CAM_SIZE_Y;

        bool _xIsBeneathWanted = _finalSizeX < WANTED_CAM_SIZE_X;
        bool _yIsBeneathWanted = _finalSizeY < WANTED_CAM_SIZE_Y;
        if (_xIsBeneathWanted || _yIsBeneathWanted)
        {
            Vector3 _tlWanted = new(-WANTED_CAM_SIZE_X / 2f, WANTED_CAM_SIZE_Y / 2f);
            Vector3 _trWanted = new(WANTED_CAM_SIZE_X / 2f, WANTED_CAM_SIZE_Y / 2f);
            Vector3 _blWanted = new(-WANTED_CAM_SIZE_X / 2f, -WANTED_CAM_SIZE_Y / 2f);
            Vector3 _brWanted = new(WANTED_CAM_SIZE_X / 2f, -WANTED_CAM_SIZE_Y / 2f);

            Vector3 _tlCurrent = new(-_finalSizeX / 2f, _finalSizeY / 2f);
            Vector3 _trCurrent = new(_finalSizeX / 2f, _finalSizeY / 2f);
            Vector3 _blCurrent = new(-_finalSizeX / 2f, -_finalSizeY / 2f);
            Vector3 _brCurrent = new(_finalSizeX / 2f, -_finalSizeY / 2f);

            UfEditor.DrawSquareGizmos(transform.position, WANTED_CAM_SIZE_X, WANTED_CAM_SIZE_Y, Color.yellow);
            UfEditor.DrawArrow(transform.position + _tlWanted, transform.position + _tlCurrent, Color.yellow, 1);
            UfEditor.DrawArrow(transform.position + _trWanted, transform.position + _trCurrent, Color.yellow, 1);
            UfEditor.DrawArrow(transform.position + _blWanted, transform.position + _blCurrent, Color.yellow, 1);
            UfEditor.DrawArrow(transform.position + _brWanted, transform.position + _brCurrent, Color.yellow, 1);
        }

        UfEditor.DrawSquareGizmos(transform.position, _finalSizeX, _finalSizeY, Color.blue);

        float _highestHorizontalSize_PC = _finalSizeY / HIGHEST_HORIZONTAL_RATIO_PC;

        float _highestHorizontalSize_Mobile = _finalSizeY / HIGHEST_HORIZONTAL_RATIO_MOBILE;
        float _highestVerticalSize = _finalSizeX * HIGHEST_VERTICAL_RATIO;

        float _differenceWithFinalSizeX_PC = _highestHorizontalSize_PC - _finalSizeX;

        float _differenceWithFinalSizeX_Mobile = _highestHorizontalSize_Mobile - _finalSizeX;
        float _differenceWithFinalSizeY = _highestVerticalSize - _finalSizeY;

        UfEditor.DrawSquareGizmos(transform.position, _gameZoneSize.x + _differenceWithFinalSizeX_PC, _gameZoneSize.y, UfColor.Orange);

        UfEditor.DrawSquareGizmos(transform.position, _gameZoneSize.x + _differenceWithFinalSizeX_Mobile, _gameZoneSize.y + _differenceWithFinalSizeY, Color.magenta);

        float _finalBorderSize = _xIsBeneathWanted ? (SIDE_BORDER_SIZE / WANTED_CAM_SIZE_X) * _finalSizeX : SIDE_BORDER_SIZE;
        float _finalTopAndBottomBorderSize = _yIsBeneathWanted ? (TOP_AND_BOTTOM_BORDER_SIZE / WANTED_CAM_SIZE_Y) * _finalSizeY : TOP_AND_BOTTOM_BORDER_SIZE;

        UfEditor.DrawBarredSquareGizmos(transform.position + new Vector3(0, -transform.localScale.y / 2 + _finalTopAndBottomBorderSize / 2), transform.localScale.x, _finalTopAndBottomBorderSize, Color.red, _resolution: 1, _lineRotation: 45);
        UfEditor.DrawBarredSquareGizmos(transform.position + new Vector3(0, transform.localScale.y / 2 - _finalTopAndBottomBorderSize / 2), transform.localScale.x, _finalTopAndBottomBorderSize, Color.red, _resolution: 1, _lineRotation: 45);
        UfEditor.DrawBarredSquareGizmos(transform.position + new Vector3(-transform.localScale.x / 2 + _finalBorderSize / 2, 0), _finalBorderSize, transform.localScale.y - _finalTopAndBottomBorderSize * 2, Color.red, _resolution: 1, _lineRotation: 45);
        UfEditor.DrawBarredSquareGizmos(transform.position + new Vector3(transform.localScale.x / 2 - _finalBorderSize / 2, 0), _finalBorderSize, transform.localScale.y - _finalTopAndBottomBorderSize * 2, Color.red, _resolution: 1, _lineRotation: 45);
    }
#endif

    protected override void GameElementFirstInitialize()
    {
        startPosition = transform.position;
    }
    protected override void GameElementEnableAndReset()
    {
        transform.position = startPosition;
    }
    protected override void GameElementPlay() { }
    protected override void GameElementUpdate() { }
}
