using System.Collections;
using System.Collections.Generic;
using Umeshu.USystem;
using Umeshu.USystem.GameCameraManager;
using UnityEngine;

public class GameSetup : MonoBehaviour
{

    [SerializeField] private Ball ball;
    public Ball Ball => ball;
    public DisplayScore displayScore;
    [SerializeField] private Level_GameZone level_GameZone;
    public Level_GameZone Level_GameZone => level_GameZone;

    void Start()
    {
        GameCameraManager.Instance.SetBallAndGameZone(Ball, Level_GameZone);
        displayScore.Init(Ball.GetComponent<ScoreBall>());
    }
}
