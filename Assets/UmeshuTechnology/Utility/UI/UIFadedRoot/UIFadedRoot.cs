using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class UIFadedRoot : HeritableGameElement
{
    private CanvasGroup canvasGroup;
    private IUIFadedRootTrigger[] uIFadedRootTriggers;

    #region Game Element Methods

    protected override void GameElementFirstInitialize()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        uIFadedRootTriggers = GetComponents<IUIFadedRootTrigger>();
    }


    protected override void GameElementEnableAndReset()
    {
        canvasGroup.alpha = 0;
    }

    protected override void GameElementPlay() { }

    protected override void GameElementUpdate()
    {
        bool _uiIsDisplayed = true;
        foreach (IUIFadedRootTrigger _uIFadedRootTrigger in uIFadedRootTriggers)
            if (!_uIFadedRootTrigger.UIIsDisplayed)
                _uiIsDisplayed = false;

        canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, _uiIsDisplayed ? 1 : 0, Time.deltaTime * 10);
        canvasGroup.blocksRaycasts = _uiIsDisplayed;
        canvasGroup.interactable = _uiIsDisplayed;
    }


    #endregion
}