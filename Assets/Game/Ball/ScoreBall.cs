using System;
using UnityEngine;

public class ScoreBall : MonoBehaviour
{
    private uint maxScore = 5;
    private uint score = 0;
    public uint Score => score;

    public Action<uint> OnScoreChange;

    private void OnCollisionEnter2D(Collision2D _other)
    {
        if (_other.gameObject.TryGetComponent(out ScoreArea _scoreArea))
        {
            AddScore(_scoreArea.scoreAmount);
            Debug.Log("Score: " + score);
        }
    }

    private void AddScore(int scoreAmount)
    {
        int _scoreTarget = (int)score + scoreAmount;
        _scoreTarget = Math.Clamp(_scoreTarget, 0, (int)maxScore);
        if (score != _scoreTarget)
        {
            if (_scoreTarget < 0) _scoreTarget = 0;
            score = (uint)_scoreTarget;
            score = Math.Clamp(score, 0, maxScore);

            OnScoreChange?.Invoke(score);
        }
    }
}