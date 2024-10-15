using TMPro;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [SerializeField] private Transform root;
    [SerializeField] private Transform textRoot;
    [SerializeField] private TextMeshPro textMeshPro;
    private Collider2D col2D;
    private uint scoreToOpen = 10;
    private uint currentScore = 0;

    private void Awake()
    {
        col2D = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Close();
    }

    private void OnCollisionEnter2D(Collision2D _other)
    {
        if (_other.gameObject.TryGetComponent(out ScoreBall _scoreBall))
        {
            int _differenceToOpen = (int)(scoreToOpen - currentScore);
            if (_differenceToOpen <= 0) return;

            currentScore += _scoreBall.Score;
            currentScore = (uint)Mathf.Clamp(currentScore, 0, scoreToOpen);
            UpdateText();

            if (_scoreBall.Score > _differenceToOpen) _scoreBall.AddScore(-_differenceToOpen);
            else _scoreBall.AddScore((int)-_scoreBall.Score);

            if (currentScore >= scoreToOpen) Open();
        }
    }

    private void UpdateText()
    {
        if (textMeshPro != null) textMeshPro.text = $"{scoreToOpen - currentScore}";
        if (textRoot != null)
        {
            textRoot.gameObject.SetActive(false);
            textRoot.gameObject.SetActive(true);
        }
    }

    private void Open()
    {
        col2D.enabled = false;
        if(root != null) root.gameObject.SetActive(false);
    }

    private void Close()
    {
        col2D.enabled = true;
        if(root != null) root.gameObject.SetActive(true);
        currentScore = 0;
        UpdateText();
    }
}
