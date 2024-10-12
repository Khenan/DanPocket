using TMPro;
using UnityEngine;

public class DisplayScore : MonoBehaviour
{
    [SerializeField] private TextMeshPro scoreText;
    [SerializeField] private Transform root;
    ScoreBall scoreBall;

    public void Init(ScoreBall _scoreBall)
    {
        scoreBall = _scoreBall;
        scoreBall.OnScoreChange -= UpdateScore;
        scoreBall.OnScoreChange += UpdateScore;
    }

    private void OnEnable()
    {
        if (scoreBall != null)
        {
            scoreBall.OnScoreChange -= UpdateScore;
            scoreBall.OnScoreChange += UpdateScore;
        }
    }
    private void OnDisable()
    {

        if (scoreBall != null)
        {
            scoreBall.OnScoreChange -= UpdateScore;
        }
    }

    public void UpdateScore(uint _score)
    {
        if (root != null)
        {
            root.gameObject.SetActive(false);
            root.gameObject.SetActive(true);
        }
        scoreText.text = _score.ToString();
    }
}