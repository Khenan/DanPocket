using TMPro;
using Umeshu.Utility;
using UnityEngine;

public class RectTransformFollowTextMeshProUGUISize : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private OptionalVar<float> updateXWithOffset = new(_initialValue: 0, _initialEnabled: false);
    [SerializeField] private OptionalVar<float> updateYWithOffset = new(_initialValue: 0, _initialEnabled: false);
    [SerializeField] private OptionalVar<float> positionXOffset = new(_initialValue: 0, _initialEnabled: false);
    [SerializeField] private OptionalVar<float> positionYOffset = new(_initialValue: 0, _initialEnabled: false);

    private void Update()
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (rectTransform == null || textMeshProUGUI == null)
            return;

        // Get the TextMeshProUGUI's rendered bounds
        Bounds _textBounds = textMeshProUGUI.textBounds;

        // Calculate the offset
        Vector3 _offset = textMeshProUGUI.transform.TransformPoint(_textBounds.center) - textMeshProUGUI.rectTransform.position;

        _offset += new Vector3(
            positionXOffset.Enabled ? positionXOffset.Value : 0,
            positionYOffset.Enabled ? positionYOffset.Value : 0,
            0);

        Vector2 _size = new(_textBounds.size.x, _textBounds.size.y);

        _size += new Vector2(
            updateXWithOffset.Enabled ? updateXWithOffset.Value : 0,
            updateYWithOffset.Enabled ? updateYWithOffset.Value : 0);

        // Update the position and size of the rectTransformToFollow
        rectTransform.position = textMeshProUGUI.rectTransform.position + _offset;
        rectTransform.sizeDelta = _size;
    }

}
