using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject loadingSprite;

    private void Update()
    {
        if (GameManager.I.GameState == GameState.LOADING)
        {
            if (loadingScreen != null && !loadingScreen.activeSelf)
            {
                loadingScreen.SetActive(true);
            }
            if (loadingSprite != null)
            {
                loadingSprite.transform.Rotate(Vector3.forward, -1f * Time.deltaTime * 360f);
            }
        }
        else
        {
            if (loadingScreen != null && loadingScreen.activeSelf)
            {
                loadingScreen.SetActive(false);
            }
        }
    }
}
